namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>template-block-body-child</c> production.
/// </summary>
internal abstract record TemplateBlockBodyChildSyntax : ISyntax
{
    public abstract void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor;
    public override String ToString() => this.ToAstString();
}
