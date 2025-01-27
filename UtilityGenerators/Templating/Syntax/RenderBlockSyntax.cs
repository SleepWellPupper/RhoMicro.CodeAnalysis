namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>render-block</c> production.
/// </summary>
/// <param name="RenderBlockHead">
/// Represents the <c>render-block-head</c> part of the production.
/// </param>
/// <param name="RenderBlockBody">
/// Represents the <c>[render-block-body]</c> part of the production.
/// </param>
internal sealed record RenderBlockSyntax(RenderBlockHeadSyntax RenderBlockHead, RenderBlockBodySyntax? RenderBlockBody = null) : BlockSyntax
{
    public const String Production = "code-body-child";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();

}
