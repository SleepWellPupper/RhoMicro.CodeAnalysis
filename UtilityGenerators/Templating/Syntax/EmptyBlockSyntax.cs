namespace RhoMicro.CodeAnalysis.Templating.Syntax;

/// <summary>
/// Represents the <c>empty-block</c> production.
/// </summary>
/// <param name="OpenBlock">
/// Represents the <c>open-block</c> part of the production.
/// </param>
/// <param name="CloseBlock">
/// Represents the <c>close-block</c> part of the production.
/// </param>
internal sealed record EmptyBlockSyntax(OpenBlockSyntax OpenBlock, CloseBlockSyntax CloseBlock) : EscapedTextSyntax
{
    public const String Production = "empty-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
