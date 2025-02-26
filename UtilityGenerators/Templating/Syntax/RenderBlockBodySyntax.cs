namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>render-block-body</c> production.
/// </summary>
/// <param name="RenderBlockTrivia">
/// Represents the <c>[render-block-trivia]</c> part of the production.
/// </param>
/// <param name="TemplateBlock">
/// Represents the <c>template-block</c> part of the production.
/// </param>
internal sealed record RenderBlockBodySyntax(RenderBlockTriviaSyntax? RenderBlockTrivia, TemplateBlockSyntax TemplateBlock) : ISyntax
{
    public const String Production = "render-block-body";

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
