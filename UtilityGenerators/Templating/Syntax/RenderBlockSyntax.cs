namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>render-block</c> production.
/// </summary>
/// <param name="RenderBlockHead">
/// Represents the <c>render-block-head</c> part of the production.
/// </param>
/// <param name="RenderBlockBody">
/// Represents the <c>[render-block-body]</c> part of the production.
/// </param>
internal sealed record RenderBlockSyntax(RenderBlockHeadSyntax RenderBlockHead, RenderBlockBodySyntax? RenderBlockBody) : ISyntax
{
    public const String Production = "render-block";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);

}
