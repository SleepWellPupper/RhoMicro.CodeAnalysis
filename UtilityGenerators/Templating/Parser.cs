// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax;

using static Diagnostic;
using static RhoMicro.CodeAnalysis.Templating.TokenKind;

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
            ? new(s)
            : new(new EmptyTemplateSyntax());

        if(!IsAtEnd())
            EmitDiagnostic(Ids.UnexpectedToken, DiagnosticSeverity.Error);

        return result;
    }
    private Boolean NonEmptyTemplate([NotNullWhen(true)] out NotEmptyTemplateSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

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

        if(!TemplateBlockBodyChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<TemplateBlockBodyChildSyntax>();
        children.Add(firstChild);

        while(TemplateBlockBodyChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new TemplateBlockBodySyntax(children);
        return true;
    }
    private Boolean TemplateBlockBodyChild([NotNullWhen(true)] out TemplateBlockBodyChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(RenderBlock(out var renderBlock))
        {
            syntax = new TemplateBlockBodyChildSyntax(renderBlock);
            return true;
        }

        if(CodeBlock(out var codeBlock))
        {
            syntax = new TemplateBlockBodyChildSyntax(codeBlock);
            return true;
        }

        if(Text(out var text))
        {
            syntax = new TemplateBlockBodyChildSyntax(text);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CodeBlock([NotNullWhen(true)] out CodeBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        var leadingTrivia = LeadingTrivia(out var l) ? l : null;

        if(!OpenCodeBlock(out var openCodeBlock))
        {
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        if(!CodeBlockBody(out var codeBlockBody))
        {
            Return(openCodeBlock);
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        if(!CloseCodeBlock(out var closeCodeBlock))
        {
            Return(codeBlockBody);
            Return(openCodeBlock);
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        var trailingTrivia = TrailingTrivia(out var t) ? t : null;

        syntax = new CodeBlockSyntax(
            leadingTrivia,
            openCodeBlock,
            codeBlockBody,
            closeCodeBlock,
            trailingTrivia);
        return true;
    }
    private Boolean CodeBlockBody([NotNullWhen(true)] out CodeBlockBodySyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!CodeBlockBodyChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<CodeBlockBodyChildSyntax>();
        children.Add(firstChild);

        while(CodeBlockBodyChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new CodeBlockBodySyntax(children);
        return true;
    }
    private Boolean CodeBlockBodyChild([NotNullWhen(true)] out CodeBlockBodyChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(RenderBlock(out var renderBlock))
        {
            syntax = new CodeBlockBodyChildSyntax(renderBlock);
            return true;
        }

        if(Text(out var text))
        {
            syntax = new CodeBlockBodyChildSyntax(text);
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

        var renderBlockBody = RenderBlockBody(out var b) ? b : null;
        syntax = new RenderBlockSyntax(renderBlockHead, renderBlockBody);
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

        syntax = new RenderBlockHeadSyntax(openRenderBlock, text, closeRenderBlock);
        return true;
    }
    private Boolean RenderBlockBody([NotNullWhen(true)] out RenderBlockBodySyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        var renderBlockTrivia = RenderBlockTrivia(out var t) ? t : null;

        if(!TemplateBlock(out var templateBlock))
        {
            Return(renderBlockTrivia);
            syntax = null;
            return false;
        }

        syntax = new RenderBlockBodySyntax(renderBlockTrivia, templateBlock);
        return true;
    }
    private Boolean RenderBlockTrivia([NotNullWhen(true)] out RenderBlockTriviaSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Newline(out var newline))
        {
            syntax = new RenderBlockTriviaSyntax(newline);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean TemplateBlock([NotNullWhen(true)] out TemplateBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        var leadingTrivia = LeadingTrivia(out var l) ? l : null;

        if(!OpenTemplateBlock(out var openTemplateBlock))
        {
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        if(!TemplateBlockBody(out var templateBlockBody))
        {
            Return(openTemplateBlock);
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        if(!CloseTemplateBlock(out var closeTemplateBlock))
        {
            Return(templateBlockBody);
            Return(openTemplateBlock);
            Return(leadingTrivia);
            syntax = null;
            return false;
        }

        var trailingTrivia = TrailingTrivia(out var t) ? t : null;

        syntax = new TemplateBlockSyntax(
            leadingTrivia,
            openTemplateBlock,
            templateBlockBody,
            closeTemplateBlock,
            trailingTrivia);
        return true;
    }
    private Boolean LeadingTrivia([NotNullWhen(true)] out LeadingTriviaSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Whitespaces(out var whitespaces))
        {
            syntax = new LeadingTriviaSyntax(whitespaces);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean Whitespaces([NotNullWhen(true)] out WhitespacesSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.Whitespaces, out var token))
        {
            syntax = new WhitespacesSyntax(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean TrailingTrivia([NotNullWhen(true)] out TrailingTriviaSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Newline(out var newline))
        {
            syntax = new TrailingTriviaSyntax(newline);
            return true;
        }

        syntax = null;
        return false;
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

        syntax = new TextSyntax(children);
        return true;
    }
    private Boolean TextChild([NotNullWhen(true)] out TextChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(EscapedText(out var escapedText))
        {
            syntax = new TextChildSyntax(escapedText);
            return true;
        }

        if(NotEscapedText(out var notEscapedText))
        {
            syntax = new TextChildSyntax(notEscapedText);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean NotEscapedText([NotNullWhen(true)] out NotEscapedTextSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!NotEscapedTextChild(out var firstChild))
        {
            syntax = null;
            return false;
        }

        var children = _collectionFactory.CreateList<NotEscapedTextChildSyntax>();
        children.Add(firstChild);

        while(NotEscapedTextChild(out var child))
        {
            _ct.ThrowIfCancellationRequested();

            children.Add(child);
        }

        syntax = new NotEscapedTextSyntax(children);
        return true;
    }
    private Boolean NotEscapedTextChild([NotNullWhen(true)] out NotEscapedTextChildSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(NotNewline(out var notNewline))
        {
            syntax = new NotEscapedTextChildSyntax(notNewline);
            return true;
        }

        if(Newline(out var newline))
        {
            syntax = new NotEscapedTextChildSyntax(newline);
            return true;
        }

        if(Whitespaces(out var whitespaces))
        {
            syntax = new NotEscapedTextChildSyntax(whitespaces);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean NotNewline([NotNullWhen(true)] out NotNewlineSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.NotNewline, out var token))
        {
            syntax = new NotNewlineSyntax(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean Newline([NotNullWhen(true)] out NewlineSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.Newline, out var token))
        {
            syntax = new NewlineSyntax(token);
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
            syntax = new EscapedTextSyntax(escapedOpenBlock);
            return true;
        }

        if(EscapedCloseBlock(out var escapedCloseBlock))
        {
            syntax = new EscapedTextSyntax(escapedCloseBlock);
            return true;
        }

        if(EmptyBlock(out var emptyBlock))
        {
            syntax = new EscapedTextSyntax(emptyBlock);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean EscapedOpenBlock([NotNullWhen(true)] out EscapedOpenBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(!OpenBlock(out var closeBlock))
        {
            syntax = null;
            return false;
        }

        if(!EscapeColon(out var escapeColon))
        {
            Return(closeBlock);
            syntax = null;
            return false;
        }

        syntax = new EscapedOpenBlockSyntax(closeBlock, escapeColon);
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

        syntax = new EscapedCloseBlockSyntax(escapeColon, closeBlock);
        return true;
    }
    private Boolean EscapeColon([NotNullWhen(true)] out EscapeColonSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(Match(TokenKind.EscapeColon, out var token))
        {
            syntax = new EscapeColonSyntax(token);
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

        syntax = new EmptyBlockSyntax(openBlock, closeBlock);
        return true;
    }
    private Boolean OpenBlock([NotNullWhen(true)] out OpenBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(OpenCodeBlock(out var openCodeBlock))
        {
            syntax = new OpenBlockSyntax(openCodeBlock);
            return true;
        }

        if(OpenRenderBlock(out var openRenderBlock))
        {
            syntax = new OpenBlockSyntax(openRenderBlock);
            return true;
        }

        if(OpenTemplateBlock(out var openTemplateBlock))
        {
            syntax = new OpenBlockSyntax(openTemplateBlock);
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
            syntax = new OpenCodeBlockSyntax(token);
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
            syntax = new OpenRenderBlockSyntax(token);
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
            syntax = new OpenTemplateBlockSyntax(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean CloseBlock([NotNullWhen(true)] out CloseBlockSyntax? syntax)
    {
        _ct.ThrowIfCancellationRequested();

        if(CloseCodeBlock(out var closeCodeBlock))
        {
            syntax = new CloseBlockSyntax(closeCodeBlock);
            return true;
        }

        if(CloseRenderBlock(out var closeRenderBlock))
        {
            syntax = new CloseBlockSyntax(closeRenderBlock);
            return true;
        }

        if(CloseTemplateBlock(out var closeTemplateBlock))
        {
            syntax = new CloseBlockSyntax(closeTemplateBlock);
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
            syntax = new CloseCodeBlockSyntax(token);
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
            syntax = new CloseRenderBlockSyntax(token);
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
            syntax = new CloseTemplateBlockSyntax(token);
            return true;
        }

        syntax = null;
        return false;
    }
    private Boolean IsAtEnd(Int32 lookahead = 0) =>
        _currentIndex + lookahead >= _tokens.Count
     || _tokens[_currentIndex + lookahead].Kind == Eof;

    private void Advance(out Token token)
    {
        token = _tokens[_currentIndex];
        Advance(1);
    }

    private void Return<TSyntax>(TSyntax? syntax)
        where TSyntax : ISyntax
    {
        if(syntax is null)
            return;

        _currentIndex -= syntax.CountTokens(_ct);
    }

    private void Advance(Int32 count) => _currentIndex += count;

    private TokenKind Peek(Int32 lookahead = 0) =>
            IsAtEnd(lookahead)
        ? (TokenKind)( -1 )
        : _tokens[_currentIndex + lookahead].Kind;

    private Boolean Peek(
        TokenKind kind,
        Int32 lookahead = 0)
    {
        if(Peek(lookahead) == kind)
            return true;

        return false;
    }
    private Boolean Peek(
        ReadOnlySpan<TokenKind> kinds,
        Int32 lookahead = 0)
    {
        var peeked = Peek(lookahead);

        foreach(var kind in kinds)
        {
            if(kind == peeked)
                return true;
        }

        return false;
    }

    private Boolean Match(
        ReadOnlySpan<TokenKind> kinds,
        out Token token,
        Int32 lookahead = 0)
    {
        if(!Peek(kinds, lookahead))
        {
            token = default;
            return false;
        }

        Advance(out token);
        return true;
    }
    private Boolean Match(
        TokenKind kind,
        out Token token,
        Int32 lookahead = 0)
    {
        if(!Peek(kind, lookahead))
        {
            token = default;
            return false;
        }

        Advance(out token);
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
