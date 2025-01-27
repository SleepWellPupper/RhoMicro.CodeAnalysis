namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>escaped-open-block</c> production.
/// </summary>
/// <param name="OpenBlock">
/// Represents the <c>open-block</c> part of the production.
/// </param>
/// <param name="EscapeColon">
/// Represents the <c>escape-colon</c> part of the production.
/// </param>
internal sealed record EscapedOpenBlockSyntax(OpenBlockSyntax OpenBlock, EscapeColonSyntax EscapeColon) : EscapedTextSyntax
{
    public const String Production = "escaped-open-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
