namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>block</c> production.
/// </summary>
internal abstract record BlockSyntax : ISyntax
{
    public override String ToString() => this.ToAstString();
    public abstract void Accept<TVisitor>(TVisitor visitor) where TVisitor : ISyntaxVisitor;
}
