namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Templating.Syntax;

[NonEquatable]
internal partial class TokenValidator(CancellationToken ct) : ISyntaxVisitor
{
    private void AssertToken<TSyntax>(Token token, TokenKind type, String lexeme, TSyntax syntax) =>
        AssertToken(token, type, l =>
        {
            if(l.Equals(lexeme))
                return (true, null);

            return (false, $"expected:\n\n{lexeme}");
        }, syntax);
    private void AssertToken<TSyntax>(Token token, TokenKind type, Func<String, (Boolean, String?)> lexemePredicate, TSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertLexeme(token, lexemePredicate, syntax);
        AssertKind(token, type, syntax);
    }
    private void AssertLexeme<TSyntax>(Token token, Func<String, (Boolean, String?)> lexemePredicate, TSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(lexemePredicate.Invoke(token.Lexeme.ToString()) is (var success, var message) && !success)
            OnError($"lexeme mismatch:\n{message}\n\nlexeme\n{token.Lexeme.ToString()}\ntoken:\n{token}\nsyntax:\n{syntax}");
    }
    private void AssertKind<TSyntax>(Token token, TokenKind kind, TSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(kind != token.Kind)
            OnError($"token kind mismatch: expected '{kind}', actual was '{token.Kind}'\ntoken:\n{token}\nsyntax:\n{syntax}");
    }

    protected virtual void OnError(String message) => throw new InvalidOperationException(message);

    void ISyntaxVisitor.Visit(CodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenCodeBlock.Accept(this);
        syntax.CodeBody.Accept(this);
        syntax.CloseCodeBlock.Accept(this);
    }
    void ISyntaxVisitor.Visit(CodeBodyChildSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        if(syntax.TryAsText(out var text))
            text.Accept(this);
        else if(syntax.TryAsRenderBlock(out var renderBlock))
            renderBlock.Accept(this);
    }
    void ISyntaxVisitor.Visit(CodeBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }

    void ISyntaxVisitor.Visit(TemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenTemplateBlock.Accept(this);
        syntax.TemplateBlockBody.Accept(this);
        syntax.CloseTemplateBlock.Accept(this);
    }

    void ISyntaxVisitor.Visit(TextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    void ISyntaxVisitor.Visit(NotEscapedTextSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertKind(syntax.Token, TokenKind.Text, syntax);
    }

    void ISyntaxVisitor.Visit(RenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.RenderBlockHead.Accept(this);
        if(syntax.RenderBlockBody is { } body)
            body.Accept(this);
    }
    void ISyntaxVisitor.Visit(RenderBlockHeadSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenRenderBlock.Accept(this);
        syntax.Text.Accept(this);
        syntax.CloseRenderBlock.Accept(this);
    }
    void ISyntaxVisitor.Visit(RenderBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.TemplateBlock.Accept(this);
    }

    public void Visit(EscapedOpenBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.EscapeColon.Accept(this);
    }
    public void Visit(EscapedCloseBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Colon.Accept(this);
        syntax.CloseBlock.Accept(this);
    }
    public void Visit(EscapeColonSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.EscapeColon, ":", syntax);
    }

    public void Visit(OpenRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenRenderBlock, "(:", syntax);
    }

    public void Visit(OpenCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenCodeBlock, "{:", syntax);
    }

    public void Visit(OpenTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenTemplateBlock, "<:", syntax);
    }

    public void Visit(CloseRenderBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseRenderBlock, ":)", syntax);
    }

    public void Visit(CloseCodeBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseCodeBlock, ":}", syntax);
    }

    public void Visit(CloseTemplateBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseTemplateBlock, ":>", syntax);
    }

    public void Visit(TriviaSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        AssertToken(
            syntax.Token,
            TokenKind.Trivia,
            s =>
            {
                if(s is not ['\n', ..] and not ['\r', '\n', ..]&& s[( s[0] is '\n' ? 1 : 2 )..].All(c => c is ' ' or '\t'))
                {
                    return (false, "expected trivia (newline followed by whitespace)");
                }

                return (true, null);
            },
            syntax);
    }

    public void Visit(BlockSequenceSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Block.Accept(this);
        foreach(var child in syntax.Blocks)
        {
            ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }
    public void Visit(TriviaBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.Trivia.Accept(this);
        syntax.Block.Accept(this);
    }

    public void Visit(EmptyBlockSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.OpenBlock.Accept(this);
        syntax.CloseBlock.Accept(this);
    }

    public void Visit(TemplateBlockBodySyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var child in syntax.Children)
        {
            ct.ThrowIfCancellationRequested();

            child.Accept(this);
        }
    }

    public void Visit(NotEmptyTemplateSyntax syntax)
    {
        ct.ThrowIfCancellationRequested();

        syntax.TemplateBlockBody.Accept(this);
    }
    public void Visit(EmptyTemplateSyntax syntax) => ct.ThrowIfCancellationRequested();
}
