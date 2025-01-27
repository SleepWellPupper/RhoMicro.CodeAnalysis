namespace RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>code-body</c> production.
/// </summary>
/// <param name="Children">
/// Represents the <c>*code-body-child</c> part of the production.
/// </param>
internal sealed record CodeBodySyntax(EquatableList<CodeBodyChildSyntax> Children) : ISyntax
{
    public const String Production = "code-body";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
