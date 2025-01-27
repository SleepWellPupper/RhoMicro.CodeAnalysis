namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

/// <summary>
/// Represents the <c>text</c> production.
/// </summary>
/// <param name="Children">
/// Represents the <c>*text-child</c> part of the production.
/// </param>
internal sealed record TextSyntax(EquatableList<TextChildSyntax> Children) : TemplateBlockBodyChildSyntax
{
    public const String Production = "code-body-child";
    public override void Accept<TVisitor>(TVisitor visitor)
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
