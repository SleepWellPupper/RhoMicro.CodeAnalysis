namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>not-newline</c> production.
/// </summary>
internal sealed record NotNewlineSyntax : ISyntax
{
    public const String Production = "not-newline";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="token">
    /// The token matching the production.
    /// </param>
    public NotNewlineSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.NotNewline);

        Token = token;
    }

    /// <summary>
    /// Gets the token matching the production.
    /// </summary>
    public Token Token { get; }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}