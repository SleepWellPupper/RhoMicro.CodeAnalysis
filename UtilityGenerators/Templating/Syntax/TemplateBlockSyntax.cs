namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>template-block</c> production.
/// </summary>
/// <param name="OpenTemplateBlock">
/// Represents the <c>open-template-block</c> part of the production.
/// </param>
/// <param name="TemplateBlockBody">
/// Represents the <c>template-block-body</c> part of the production.
/// </param>
/// <param name="CloseTemplateBlock">
/// Represents the <c>close-template-block</c> part of the production.
/// </param>
internal sealed record TemplateBlockSyntax(
    OpenTemplateBlockSyntax OpenTemplateBlock,
    TemplateBlockBodySyntax TemplateBlockBody,
    CloseTemplateBlockSyntax CloseTemplateBlock) : BlockSyntax
{
    public const String Production = "template-block";
    public override void Accept<TVisitor>(TVisitor visitor)
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
