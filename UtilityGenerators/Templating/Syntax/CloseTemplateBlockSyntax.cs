namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>close-template-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>CLN CAB</c> part of the production.
/// </param>
internal sealed record CloseTemplateBlockSyntax(Token Token) : CloseBlockSyntax(Token, TokenKind.CloseTemplateBlock)
{
    public const String Production = "close-template-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}