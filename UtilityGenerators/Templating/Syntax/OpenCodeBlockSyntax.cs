namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>open-code-block</c> production.
/// </summary>
internal sealed record OpenCodeBlockSyntax : ISyntax
{
    public const String Production = "open-code-block";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="token">
    /// The <c>OCB CLN</c> part of the production.
    /// </param>
    public OpenCodeBlockSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.OpenCodeBlock);

        Token = token;
    }

    /// <summary>
    /// Gets the <c>OCB CLN</c> part of the production.
    /// </summary>
    public Token Token { get; }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
