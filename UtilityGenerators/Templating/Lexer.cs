namespace RhoMicro.CodeAnalysis.Templating;

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
        CancellationToken ct)
    {
        _templateString = templateString;

        _startLine = templateString.Start.Line;
        _startCharacter = templateString.Start.Character;
        _currentLine = templateString.Start.Line;
        _currentCharacter = templateString.Start.Character;
        _previousEolCharacter = templateString.Start.Character - 1;

        _tokens = tokens;
        _diagnostics = diagnostics;
        _ct = ct;
    }

    private readonly EquatableList<Token> _tokens;
    private readonly EquatableList<Diagnostic> _diagnostics;
    private readonly CancellationToken _ct;

    private readonly TemplateString _templateString;

    private Int32 _startIndex;
    private Int32 _currentIndex;
    private Int32 _startLine;
    private Int32 _startCharacter;
    private Int32 _currentLine;
    private Int32 _currentCharacter;
    private Int32 _previousEolCharacter;

    private Int32 _requiredQuotesAcc = 1;
    private Int32 _requiredQuotes = 3;

    public static ScanResult Scan(TemplateString templateString, in ModelCreationContext context)
    {
        var result = new Lexer(
            templateString,
            context.CollectionFactory.CreateList<Token>(),
            context.CollectionFactory.CreateList<Diagnostic>(),
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

            SetStartToCurrent();

            ScanToken();
        }

        _tokens.Add(Token.CreateEof(_templateString, new(_currentLine, _currentCharacter)));

        var result = new ScanResult(_tokens, _templateString, _requiredQuotes, _diagnostics);

        return result;
    }

    private void SetStartToCurrent()
    {
        _startIndex = _currentIndex;
        _startCharacter = _currentCharacter;
        _startLine = _currentLine;
    }

    private void ScanToken()
    {
        _ct.ThrowIfCancellationRequested();

        var c = Advance();

        switch(c)
        {
            // open render block
            case '(' when Match(':'):
                AddOpenBlockToken(OpenRenderBlock);
                break;
            // open code block
            case '{' when Match(':'):
                AddOpenBlockToken(OpenCodeBlock);
                break;
            // open template block
            case '<' when Match(':'):
                AddOpenBlockToken(OpenTemplateBlock);
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
            // text
            default:
                CheckNewline(c);
                Text();
                break;
        }
    }
    private Boolean PeekBlockTerminator(Int32 lookahead = 0) =>
        PeekSpan(lookahead, 2) is
        // open: (: or {: or <:
        ['(' or '{' or '<', ':'] or
        // close: :) or :} or :>
        [':', ')' or '}' or '>'];
    private Boolean PeekEscape(Int32 lookahead = 0) =>
        PeekSpan(lookahead, 3) is
        // open: (:: or {:: or <::
        ['(' or '{' or '<', ':', ':'] or
        // close: ::) or ::} or ::>
        [':', ':', ')' or '}' or '>'];

    private void Text()
    {
        _ct.ThrowIfCancellationRequested();

        while(!IsAtEnd() && !PeekBlockTerminator() && !PeekEscape())
        {
            _ct.ThrowIfCancellationRequested();

            var c = Advance();
            CheckNewline(c);
        }

        AddToken(TokenKind.Text);
    }

    private void CheckNewline(Char c)
    {
        if(c is not '\n' && ( c is not '\r' || !Match('\n') ))
            return;

        // Newlines are meaningful for multiline raw string literals, as they
        // affect positions relative to the C# source text. For single-line
        // string literals, they are ordinary characters on the same source text
        // line; we do not increment the line and reset the character in this
        // case. 
        if(!_templateString.IsMultiline)
            return;

        _previousEolCharacter = _currentCharacter - 1;
        _currentLine++;
        _currentCharacter = _templateString.Start.Character;
    }
    private static Boolean IsTrivia(Token potentialTrivia)
    {
        var lexeme = potentialTrivia.Lexeme;
        var end = lexeme[0] is '\n' ? 0 : 1;
        for(var i = lexeme.Length - 1; i < end; i++)
        {
            if(lexeme[i] is not ' ' and not '\t')
                return false;
        }

        return true;
    }
    private void AddOpenBlockToken(TokenKind type)
    {
        if(_tokens is [..,
            { Kind: CloseCodeBlock or CloseRenderBlock or CloseTemplateBlock },
            { Kind: TokenKind.Text, Lexeme: ['\n', ..] or ['\r', '\n', ..] } potentialTrivia]
            && IsTrivia(potentialTrivia))
        {

            _tokens[^1] = potentialTrivia with { Kind = Trivia };
        }

        AddToken(type);
    }

    private void AddToken(TokenKind type)
    {
        var token = CreateToken(type);
        _tokens.Add(token);
    }

    private Token CreateToken(TokenKind type)
    {
        var length = _currentIndex - _startIndex;

        Debug.Assert(length > 0);

        var templateSpan = new TemplateSpan(_startIndex, length);
        var start = new SourcePosition(_startLine, _startCharacter);
        var end = _currentCharacter == _templateString.Start.Character
            ? new SourcePosition(_currentLine - 1, _previousEolCharacter)
            : new SourcePosition(_currentLine, _currentCharacter - 1);

        var sourceSpan = new SourceSpan(start, end);
        var spans = new TokenSpans(templateSpan, sourceSpan);
        var result = new Token(type, _templateString, spans);

        return result;
    }

    private Boolean IsAtEnd(Int32 lookahead = 0) => _currentIndex + lookahead >= _templateString.Text.Length;

    private Char Advance()
    {
        var result = _templateString.Text[_currentIndex];
        Advance(1);
        return result;
    }

    private void Advance(Int32 count)
    {
        if(Peek() == '"')
        {
            _requiredQuotesAcc++;
        } else
        {
            _requiredQuotes = Math.Max(_requiredQuotes, _requiredQuotesAcc);
            _requiredQuotesAcc = 1;
        }

        _currentIndex += count;
        _currentCharacter += count;
    }
    private ReadOnlySpan<Char> PeekSpan(Int32 length) => PeekSpan(0, length);
    private ReadOnlySpan<Char> PeekSpan(Int32 lookahead, Int32 length)
    {
        Debug.Assert(length > 0);

        if(IsAtEnd(lookahead + length - 1))
            return [];

        var result = _templateString.Text
            .AsSpan(_currentIndex + lookahead, length);

        return result;
    }
    private Char Peek(Int32 lookahead = 0)
    {
        if(IsAtEnd(lookahead))
            return '\0';

        var result = _templateString.Text[_currentIndex + lookahead];

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

    private String GetCurrentLexemeString() => _templateString.Text[_startIndex.._currentIndex].Replace("\r", "\\r").Replace("\n", "\\n");
}
