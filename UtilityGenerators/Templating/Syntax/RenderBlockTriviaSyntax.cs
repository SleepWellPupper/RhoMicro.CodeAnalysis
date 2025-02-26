namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>render-block-trivia</c> production.
/// </summary>
/// <param name="Newline">
/// Represents the <c>[newline]</c> part of the production.
/// </param>
internal sealed record RenderBlockTriviaSyntax(NewlineSyntax Newline) : ISyntax
{
    public const String Production = "render-block-trivia";

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
