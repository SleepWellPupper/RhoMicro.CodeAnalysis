namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>open-render-block</c> production.
/// </summary>
internal sealed record OpenRenderBlockSyntax : ISyntax
{
    public const String Production = "open-render-block";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="token">
    /// The <c>OPA CLN</c> part of the production.
    /// </param>
    public OpenRenderBlockSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.OpenRenderBlock);

        Token = token;
    }

    /// <summary>
    /// Gets the <c>OPA CLN</c> part of the production.
    /// </summary>
    public Token Token { get; }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
