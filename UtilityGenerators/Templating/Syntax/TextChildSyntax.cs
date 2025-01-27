namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>text-child</c> production.
/// </summary>
internal abstract record TextChildSyntax : ISyntax
{
    public abstract void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor;
    public override String ToString() => this.ToAstString();
}
