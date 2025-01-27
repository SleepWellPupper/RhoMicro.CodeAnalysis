namespace RhoMicro.CodeAnalysis.Templating;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax;

using static Diagnostic;
using static TokenKind;

[NonEquatable]
[DebuggerDisplay("{GetDebugDisplayString()}")]
internal sealed partial class Parser
{
    private Parser(
        EquatableCollectionFactory collectionFactory,
        EquatableList<Diagnostic> diagnostics,
        EquatableList<Token> tokens,
        CancellationToken ct)
    {
        _collectionFactory = collectionFactory;
        _diagnostics = diagnostics;
        _tokens = tokens;
        _ct = ct;
    }

    private readonly EquatableCollectionFactory _collectionFactory;
    private readonly EquatableList<Diagnostic> _diagnostics;
    private readonly EquatableList<Token> _tokens;
    private readonly CancellationToken _ct;

    private Int32 _currentIndex;

    public static ParseResult Parse(ScanResult scanResult, in ModelCreationContext context)
    {
        var (collectionFactory, ct) = context;
        var tokens = scanResult.Tokens;

        var diagnostics = collectionFactory.CreateList<Diagnostic>();

        var template = new Parser(
            collectionFactory,
            diagnostics,
            tokens,
            ct)
            .Template();

        template.DebugValidate(context.CancellationToken);

        var result = new ParseResult(template, scanResult, diagnostics);

        return result;
    }

    private TemplateSyntax Template()
    {
        _ct.ThrowIfCancellationRequested();

        TemplateSyntax result = NonEmptyTemplate(out var s)
            ? s
            : new EmptyTemplateSyntax();

        if(!IsAtEnd())
            EmitDiagnostic(Ids.UnexpectedToken, DiagnosticSeverity.Error);

        return result;
    }
    private Boolean NonEmptyTemplate([NotNullWhen(true)] out NotEmptyTemplateSyntax? syntax)
    {
        if(!TemplateBlockBody(out var templateBlockBody))
        {
            syntax = null;
            return false;
        }

        syntax = new(templateBlockBody);
        return true;
    }
    private Boolean TemplateBlockBody([NotNullWhen(true)] out TemplateBlockBodySyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!TemplateChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<TemplateBlockBodyChildSyntax>();
        children.Add(firstChild);

        while(TemplateChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new TemplateBlockBodySyntax(children);
        return true;
    }
    private Boolean TemplateChild([NotNullWhen(true)] out TemplateBlockBodyChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Text(out var text))
        {
            syntax = text;
            return true;
        }

        if(BlockSequence(out var blockSequence))
        {
            syntax = blockSequence;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean BlockSequence([NotNullWhen(true)] out BlockSequenceSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!Block(out var block))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<TriviaBlockSyntax>();

        while(NewlineBlock(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new BlockSequenceSyntax(block, children);
        return true;
    }
    private Boolean NewlineBlock([NotNullWhen(true)] out TriviaBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!Trivia(out var newlineTrivia))
        {
            syntax = null;
            return false;
        }

        if(!Block(out var block))
        {
            Return(newlineTrivia);
            syntax = null;
            return false;
        }

        syntax = new(newlineTrivia, block);
        return true;
    }
    private Boolean Trivia([NotNullWhen(true)] out TriviaSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.Trivia, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean Block([NotNullWhen(true)] out BlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(RenderBlock(out var renderBlock))
        {
            syntax = renderBlock;
            return true;
        }

        if(CodeBlock(out var codeBlock))
        {
            syntax = codeBlock;
            return true;
        }

        if(TemplateBlock(out var templateBlock))
        {
            syntax = templateBlock;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CodeBlock([NotNullWhen(true)] out CodeBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenCodeBlock(out var openCodeBlock))
        {
            syntax = null;
            return false;
        }

        if(!CodeBody(out var codeBody))
        {
            Return(openCodeBlock);
            syntax = null;
            return false;
        }

        if(!CloseCodeBlock(out var closeCodeBlock))
        {
            Return(codeBody);
            Return(openCodeBlock);
            syntax = null;
            return false;
        }

        syntax = new(openCodeBlock, codeBody, closeCodeBlock);
        return true;
    }
    private Boolean CodeBody([NotNullWhen(true)] out CodeBodySyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!CodeBodyChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<CodeBodyChildSyntax>();
        children.Add(firstChild);

        while(CodeBodyChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new(children);
        return true;
    }
    private Boolean CodeBodyChild([NotNullWhen(true)] out CodeBodyChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Text(out var text))
        {
            syntax = text;
            return true;
        }

        if(RenderBlock(out var renderBlock))
        {
            syntax = renderBlock;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean RenderBlock([NotNullWhen(true)] out RenderBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!RenderBlockHead(out var renderBlockHead))
        {
            syntax = null;
            return false;
        }

        if(RenderBlockBody(out var renderBlockBody))
        {
            syntax = new(renderBlockHead, renderBlockBody);
            return true;
        }

        syntax = new(renderBlockHead);
        return true;
    }
    private Boolean RenderBlockBody([NotNullWhen(true)] out RenderBlockBodySyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        var trivia = Trivia(out var t) ? t : null;

        if(!TemplateBlock(out var templateBlock))
        {
            if(trivia is not null)
                Return(trivia);
            syntax = null;
            return false;
        }

        syntax = new(trivia, templateBlock);
        return true;
    }
    private Boolean TemplateBlock([NotNullWhen(true)] out TemplateBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenTemplateBlock(out var openTemplateBlock))
        {
            syntax = null;
            return false;
        }

        if(!TemplateBlockBody(out var templateBlockBody))
        {
            Return(openTemplateBlock);
            syntax = null;
            return false;
        }

        if(!CloseTemplateBlock(out var closeTemplateBlock))
        {
            Return(templateBlockBody);
            Return(openTemplateBlock);
            syntax = null;
            return false;
        }

        syntax = new(openTemplateBlock, templateBlockBody, closeTemplateBlock);
        return true;
    }
    private Boolean RenderBlockHead([NotNullWhen(true)] out RenderBlockHeadSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenRenderBlock(out var openRenderBlock))
        {
            syntax = null;
            return false;
        }

        if(!Text(out var text))
        {
            Return(openRenderBlock);
            syntax = null;
            return false;
        }

        if(!CloseRenderBlock(out var closeRenderBlock))
        {
            Return(text);
            Return(openRenderBlock);
            syntax = null;
            return false;
        }

        syntax = new(openRenderBlock, text, closeRenderBlock);
        return true;
    }
    private Boolean Text([NotNullWhen(true)] out TextSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!TextChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<TextChildSyntax>();
        children.Add(firstChild);

        while(TextChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new(children);
        return true;
    }
    private Boolean TextChild([NotNullWhen(true)] out TextChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(NotEscapedText(out var notEscaped))
        {
            syntax = notEscaped;
            return true;
        }

        if(EscapedText(out var escaped))
        {
            syntax = escaped;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean NotEscapedText([NotNullWhen(true)] out NotEscapedTextSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.Text, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean EscapedText([NotNullWhen(true)] out EscapedTextSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(EscapedOpenBlock(out var escapedOpenBlock))
        {
            syntax = escapedOpenBlock;
            return true;
        }

        if(EscapedCloseBlock(out var escapedCloseBlock))
        {
            syntax = escapedCloseBlock;
            return true;
        }

        if(EmptyBlock(out var emptyBlock))
        {
            syntax = emptyBlock;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean EmptyBlock([NotNullWhen(true)] out EmptyBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenBlock(out var openBlock))
        {
            syntax = null;
            return false;
        }

        if(!CloseBlock(out var closeBlock))
        {
            Return(openBlock);
            syntax = null;
            return false;
        }

        syntax = new(openBlock, closeBlock);
        return true;
    }
    private Boolean EscapedCloseBlock([NotNullWhen(true)] out EscapedCloseBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!EscapeColon(out var escapeColon))
        {
            syntax = null;
            return false;
        }

        if(!CloseBlock(out var closeBlock))
        {
            Return(escapeColon);
            syntax = null;
            return false;
        }

        syntax = new(escapeColon, closeBlock);
        return true;
    }
    private Boolean CloseBlock([NotNullWhen(true)] out CloseBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(CloseCodeBlock(out var closeCodeBlock))
        {
            syntax = closeCodeBlock;
            return true;
        }

        if(CloseRenderBlock(out var closeRenderBlock))
        {
            syntax = closeRenderBlock;
            return true;
        }

        if(CloseTemplateBlock(out var closeTemplateBlock))
        {
            syntax = closeTemplateBlock;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CloseTemplateBlock([NotNullWhen(true)] out CloseTemplateBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.CloseTemplateBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CloseRenderBlock([NotNullWhen(true)] out CloseRenderBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.CloseRenderBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CloseCodeBlock([NotNullWhen(true)] out CloseCodeBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.CloseCodeBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean EscapedOpenBlock([NotNullWhen(true)] out EscapedOpenBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenBlock(out var openBlock))
        {
            syntax = null;
            return false;
        }

        if(!EscapeColon(out var escapeColon))
        {
            Return(openBlock);
            syntax = null;
            return false;
        }

        syntax = new(openBlock, escapeColon);
        return true;
    }
    private Boolean EscapeColon([NotNullWhen(true)] out EscapeColonSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.EscapeColon, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean OpenBlock([NotNullWhen(true)] out OpenBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(OpenCodeBlock(out var openCodeBlock))
        {
            syntax = openCodeBlock;
            return true;
        }

        if(OpenRenderBlock(out var openRenderBlock))
        {
            syntax = openRenderBlock;
            return true;
        }

        if(OpenTemplateBlock(out var openTemplateBlock))
        {
            syntax = openTemplateBlock;
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean OpenTemplateBlock([NotNullWhen(true)] out OpenTemplateBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.OpenTemplateBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean OpenRenderBlock([NotNullWhen(true)] out OpenRenderBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.OpenRenderBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean OpenCodeBlock([NotNullWhen(true)] out OpenCodeBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.OpenCodeBlock, out var token))
        {
            syntax = new(token);
            return true;
        }

        syntax = null;
        return false;
    }

    private Boolean IsAtEnd(Int32 lookahead = 0) =>
        _currentIndex + lookahead >= _tokens.Count
     || _tokens[_currentIndex + lookahead].Kind == Eof;

    private Token Advance()
    {
        var result = _tokens[_currentIndex];
        Advance(1);
        return result;
    }

    private void Return<TSyntax>(TSyntax syntax)
        where TSyntax : ISyntax
        => _currentIndex -= syntax.CountTokens(_ct);

    private void Advance(Int32 count) => _currentIndex += count;

    private TokenKind Peek(Int32 lookahead = 0) =>
            IsAtEnd(lookahead)
        ? (TokenKind)( -1 )
        : _tokens[_currentIndex + lookahead].Kind;

    private Boolean Peek(
        TokenKind type,
        Int32 lookahead = 0)
    {
        if(Peek(lookahead) == type)
            return true;

        return false;
    }

    private Boolean Match(
        TokenKind type,
        [NotNullWhen(true)] out Token? token,
        Int32 lookahead = 0)
    {
        if(!Peek(type, lookahead))
        {
            token = null;
            return false;
        }

        token = Advance();
        return true;
    }

    private void EmitDiagnostic(String id, DiagnosticSeverity severity)
    {
        var token = _tokens[_currentIndex];
        var path = token.TemplateString.Path;
        var sourceSpan = token.Spans.SourceSpan;
        _diagnostics.Add(Create(id, severity, path, sourceSpan));
    }

    private String GetDebugDisplayString() =>
        String.Join(
            ";",
            _tokens
            .Skip(_currentIndex)
            .Take(4)
            .Select(t => $"{t.Kind}({t.Lexeme.ToString()})"));
}

file static class Extensions
{
    public static void Deconstruct(
        in this ModelCreationContext ctx,
        out EquatableCollectionFactory collectionFactory,
        out CancellationToken ct)
    {
        collectionFactory = ctx.CollectionFactory;
        ct = ctx.CancellationToken;
    }
}