namespace RhoMicro.CodeAnalysis.Templating.Syntax;

/// <summary>
/// Represents the <c>empty-template</c> production.
/// </summary>
internal sealed record EmptyTemplateSyntax : TemplateSyntax
{
    public const String Production = "empty-template";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
