namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>close-block</c> production.
/// </summary>
internal abstract record CloseBlockSyntax : ISyntax
{
    protected CloseBlockSyntax(Token token, TokenKind kind)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, kind);
        Token = token;
    }
    /// <summary>
    /// Gets the matched token.
    /// </summary>
    public Token Token { get; }
    public abstract void Accept<TVisitor>(TVisitor visitor) where TVisitor : ISyntaxVisitor;
    public override String ToString() => this.ToAstString();
}
