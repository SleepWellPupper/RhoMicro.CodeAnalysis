namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>escaped-open-block</c> production.
/// </summary>
/// <param name="OpenBlock">
/// Represents the <c>open-block</c> part of the production.
/// </param>
/// <param name="EscapeColon">
/// Represents the <c>escape-colon</c> part of the production.
/// </param>
internal sealed record EscapedOpenBlockSyntax(OpenBlockSyntax OpenBlock, EscapeColonSyntax EscapeColon) : ISyntax
{
    public const String Production = "escaped-open-block";
    public void Accept<TVisitor>(TVisitor visitor) 
        where TVisitor:ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
