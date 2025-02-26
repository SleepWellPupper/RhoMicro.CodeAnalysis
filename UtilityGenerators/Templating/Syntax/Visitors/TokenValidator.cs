namespace RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

using System;
using System.Threading;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

internal partial class TokenValidator(CancellationToken ct) : TreeWalkingSyntaxVisitor(ct)
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
        Ct.ThrowIfCancellationRequested();

        AssertLexeme(token, lexemePredicate, syntax);
        AssertKind(token, type, syntax);
    }
    private void AssertLexeme<TSyntax>(Token token, Func<String, (Boolean, String?)> lexemePredicate, TSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        if(lexemePredicate.Invoke(token.Lexeme.ToString()) is (var success, var message) && !success)
            OnError($"lexeme mismatch:\n{message}\n\nlexeme\n{token.Lexeme.ToString()}\ntoken:\n{token}\nsyntax:\n{syntax}");
    }
    private void AssertKind<TSyntax>(Token token, TokenKind kind, TSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        if(kind != token.Kind)
            OnError($"token kind mismatch: expected '{kind}', actual was '{token.Kind}'\ntoken:\n{token}\nsyntax:\n{syntax}");
    }

    protected virtual void OnError(String message) => throw new InvalidOperationException(message);

    public override void Visit(EscapeColonSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.EscapeColon, ":", syntax);
    }

    public override void Visit(OpenRenderBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenRenderBlock, "(:", syntax);
    }

    public override void Visit(OpenCodeBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenCodeBlock, "{:", syntax);
    }

    public override void Visit(OpenTemplateBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.OpenTemplateBlock, "<:", syntax);
    }

    public override void Visit(CloseRenderBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseRenderBlock, ":)", syntax);
    }

    public override void Visit(CloseCodeBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseCodeBlock, ":}", syntax);
    }

    public override void Visit(CloseTemplateBlockSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(syntax.Token, TokenKind.CloseTemplateBlock, ":>", syntax);
    }

    public override void Visit(NewlineSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(
            syntax.Token,
            TokenKind.Newline,
            static s =>
            (
                s is ['\n' or '\r'] or ['\r', '\n'],
                "Expected '\\r', '\\n' or '\\r\\n'"
            ),
            syntax);
    }

    public override void Visit(NotNewlineSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(
            syntax.Token,
            TokenKind.NotNewline,
            static s => (true, null),
            syntax);
    }

    public override void Visit(WhitespacesSyntax syntax)
    {
        Ct.ThrowIfCancellationRequested();

        AssertToken(
            syntax.Token,
            TokenKind.Whitespaces,
            static s => (true, null),
            syntax);
    }
}
