namespace RhoMicro.CodeAnalysis.Templating.Syntax;

/// <summary>
/// Represents the <c>not-empty-template</c> production.
/// </summary>
/// <param name="TemplateBlockBody">
/// Represents the <c>template-block-body</c> part of the production.
/// </param>
internal sealed record NotEmptyTemplateSyntax(TemplateBlockBodySyntax TemplateBlockBody) : TemplateSyntax
{
    public const String Production = "not-empty-template";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
