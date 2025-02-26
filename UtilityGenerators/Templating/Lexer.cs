namespace RhoMicro.CodeAnalysis.Templating;

using System.Buffers;
using System.Diagnostics;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

using static RhoMicro.CodeAnalysis.Templating.TokenKind;

/// <summary>
/// Scans template strings for tokens.
/// </summary>
[NonEquatable]
[DebuggerDisplay("{GetCurrentLexemeString()}")]
internal sealed partial class Lexer
{
    private Lexer(
        TemplateString templateString,
        EquatableList<Token> tokens,
        EquatableList<Diagnostic> diagnostics,
        Int32 newlineLength,
        CancellationToken ct)
    {
        _templateString = templateString;
        _reifiedTemplateString = templateString.Text.ToString();

        _line = templateString.Start.Line;
        _char = templateString.Start.Character;

        _tokens = tokens;
        _diagnostics = diagnostics;
        _ct = ct;

        _newlineLength = newlineLength;
    }

    private readonly EquatableList<Token> _tokens;
    private readonly EquatableList<Diagnostic> _diagnostics;
    private readonly CancellationToken _ct;

    private readonly TemplateString _templateString;
    private readonly String _reifiedTemplateString;

    private readonly Int32 _newlineLength;

    private Int32 _newlineOffset;
    private Int32 _index;
    private Int32 _length;
    private Int32 _line;
    private Int32 _char;

    private Int32 _requiredQuotesAcc = 1;
    private Int32 _requiredQuotes = 3;

    public static ScanResult Scan(TemplateString templateString, Int32 newlineLength, in ModelCreationContext context)
    {
        var result = new Lexer(
            templateString,
            context.CollectionFactory.CreateList<Token>(),
            context.CollectionFactory.CreateList<Diagnostic>(),
            newlineLength,
            context.CancellationToken)
            .Scan();

        return result;
    }

    private ScanResult Scan()
    {
        _ct.ThrowIfCancellationRequested();

        while(!IsAtEnd())
        {
            _ct.ThrowIfCancellationRequested();
            ScanToken();
            ResetIndices();
        }

        AppendEof();

        var result = new ScanResult(_tokens, _templateString, _requiredQuotes, _diagnostics);

        return result;
    }

    private void AppendEof()
    {
        var eofEnd = _tokens is [..,
        {
            Kind: Newline,
            Spans.SourceSpan.End:
            {
                Line: var line,
                Character: var character
            }
        }]
            ? new SourcePosition(line, character + 1)
            : new SourcePosition(_line, _char);

        _tokens.Add(Token.CreateEof(_templateString, eofEnd, _newlineOffset));
    }

    private void ResetIndices()
    {
        _char += _length;
        _index += _length;
        _length = 0;
    }

    private void ScanToken()
    {
        _ct.ThrowIfCancellationRequested();

        var c = Advance();

        switch(c)
        {
            // open render block
            case '(' when Match(':'):
                AddToken(OpenRenderBlock);
                break;
            // open code block
            case '{' when Match(':'):
                AddToken(OpenCodeBlock);
                break;
            // open template block
            case '<' when Match(':'):
                AddToken(OpenTemplateBlock);
                break;
            // close render block
            case ':' when Match(')'):
                AddToken(CloseRenderBlock);
                break;
            // close code block
            case ':' when Match('}'):
                AddToken(CloseCodeBlock);
                break;
            // close template block
            case ':' when Match('>'):
                AddToken(CloseTemplateBlock);
                break;
            // escaped block
            case ':' when
                // escaped close block
                PeekSpan(2) is [':', ')' or '}' or '>'] ||
                // escaped open block
                _tokens is [.., { Kind: OpenCodeBlock or OpenRenderBlock or OpenTemplateBlock }]:
                AddToken(EscapeColon);
                break;
            // \r[\n]
            case '\r':
                // attempt to consume a possible \n as well
                _ = Match('\n');
                AddNewline();
                break;
            case '\n':
                AddNewline();
                break;
            // text / whitespace
            default:
                var isWhitespace = c is ' ' or '\t';

                while(!( IsAtEnd() ||
                        PeekSpan(3) is
                        // \n / \r
                        ['\n' or '\r', ..] or
                        // \r\n is implied by \n / \r
                        // ['\r', '\n', ..] or
                        // open: (: or {: or <:
                        ['(' or '{' or '<', ':', .. /*discard rest to match any beginning with open*/] or
                        // close: :) or :} or :>
                        [':', ')' or '}' or '>', .. /*discard rest to match any beginning with close*/] or
                        // open: (:: or {:: or <::
                        ['(' or '{' or '<', ':', ':'] or
                        // close: ::) or ::} or ::>
                        [':', ':', ')' or '}' or '>'] ))
                {
                    _ct.ThrowIfCancellationRequested();
                    isWhitespace &= Advance() is ' ' or '\t';
                }

                var kind = isWhitespace
                    ? Whitespaces
                    : NotNewline;

                AddToken(kind);
                break;
        }
    }

    private void AddNewline()
    {
        AddToken(Newline);
        _line++;
        _char = _templateString.Start.Character;
        _index += _length;
        var newlineDelta = _newlineLength - _length;
        _newlineOffset += newlineDelta;
        _length = 0;
    }
    private void AddToken(TokenKind kind)
    {
        CreateToken(kind, out var token);
        _tokens.Add(token);
    }
    private void CreateToken(TokenKind kind, out Token token)
    {
        Debug.Assert(_length > 0);

        var newlineAwareLength = kind is TokenKind.Newline
            ? _newlineLength
            : _length;
        var newlineAwareIndex = _index + _newlineOffset;

        var templateSpan = new TemplateSpan(_index, _length);
        var newlineAwareTemplateSpan = new TemplateSpan(newlineAwareIndex, newlineAwareLength);
        var start = new SourcePosition(_line, _char);
        var end = new SourcePosition(_line, _char + _length - 1);

        var sourceSpan = new SourceSpan(start, end);
        var spans = new TokenSpans(
            TemplateSpan: templateSpan,
            NewlineAwareTemplateSpan: newlineAwareTemplateSpan,
            sourceSpan);
        token = new Token(kind, _templateString, spans);
    }
    private Int32 Remaining() => _reifiedTemplateString.Length - _index - _length;
    private Boolean IsAtEnd(Int32 lookahead = 0) => lookahead >= Remaining();
    private Char Advance()
    {
        var result = _reifiedTemplateString[_index + _length];
        Advance(1);
        return result;
    }
    private void Advance(Int32 count)
    {
        for(var i = 0; i < count; i++)
        {
            if(Peek(i) == '"')
            {
                _requiredQuotesAcc++;
            } else
            {
                _requiredQuotes = Math.Max(_requiredQuotes, _requiredQuotesAcc);
                _requiredQuotesAcc = 1;
            }
        }

        _length += count;
    }
    private ReadOnlySpan<Char> PeekSpan(Int32 maxLength) => PeekSpan(0, maxLength);
    private ReadOnlySpan<Char> PeekSpan(Int32 lookahead, Int32 maxLength)
    {
        Debug.Assert(maxLength > 0);

        ReadOnlySpan<Char> result;

        if(IsAtEnd(lookahead))
        {
            // The remainder after lookahead is empty.
            result = [];
        } else
        {
            // We obtain the remainder after lookahead.
            var remaining = _reifiedTemplateString.AsSpan(_index + _length + lookahead);
            // We return either the remainder, or an exactly sized slice,
            // depending on which is smaller.
            result = remaining.Length <= maxLength
                ? remaining
                : remaining[..maxLength];
        }

        return result;
    }
    private Char Peek(Int32 lookahead = 0)
    {
        if(IsAtEnd(lookahead))
            return '\0';

        var result = _reifiedTemplateString[_index + _length + lookahead];

        return result;
    }
    private Boolean Match(Char c, Int32 lookahead = 0)
    {
        if(Peek(lookahead) is { } p && p == c)
        {
            Advance(lookahead + 1);
            return true;
        }

        return false;
    }

    private String GetCurrentLexemeString() => _reifiedTemplateString
        .Substring(_index, _length)
        .Replace("\r", "\\r")
        .Replace("\n", "\\n");
}
