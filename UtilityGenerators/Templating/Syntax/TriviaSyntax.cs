namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>trivia</c> production.
/// </summary>
internal sealed record TriviaSyntax : ISyntax
{
    /// <summary>
    /// Represents the <c>trivia</c> production.
    /// </summary>
    /// <param name="token">
    /// The matched token.
    /// </param>
    public TriviaSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.Trivia);
        Token = token;
    }
    public const String Production = "newline-trivia";
    /// <summary>
    /// Gets the matched token.
    /// </summary>
    public Token Token { get; }
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
