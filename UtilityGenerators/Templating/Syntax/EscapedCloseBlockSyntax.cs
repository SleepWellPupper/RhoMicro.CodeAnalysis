namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>escaped-open-block</c> production.
/// </summary>
/// <param name="Colon">
/// Represents the <c>escape-colon</c> part of the production.
/// </param>
/// <param name="CloseBlock">
/// Represents the <c>close-block</c> part of the production.
/// </param>
internal sealed record EscapedCloseBlockSyntax(EscapeColonSyntax Colon, CloseBlockSyntax CloseBlock) : EscapedTextSyntax
{
    public const String Production = "escaped-close-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
