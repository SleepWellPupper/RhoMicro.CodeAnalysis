namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>render-block-body</c> production.
/// </summary>
/// <param name="Trivia">
/// Represents the <c>[trivia]</c> part of the production.
/// </param>
/// <param name="TemplateBlock">
/// Represents the <c>template-block</c> part of the production.
/// </param>
internal sealed record RenderBlockBodySyntax(TriviaSyntax? Trivia, TemplateBlockSyntax TemplateBlock) : ISyntax
{
    /// <summary>
    /// Represents the <c>render-block-body</c> production.
    /// </summary>
    /// <param name="TemplateBlock">
    /// Represents the <c>template-block</c> part of the production.
    /// </param>
#pragma warning disable IDE1006 // Naming Styles
    public RenderBlockBodySyntax(TemplateBlockSyntax TemplateBlock)
#pragma warning restore IDE1006 // Naming Styles
        : this(null, TemplateBlock)
    { }

    public const String Production = "render-block-body";

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
