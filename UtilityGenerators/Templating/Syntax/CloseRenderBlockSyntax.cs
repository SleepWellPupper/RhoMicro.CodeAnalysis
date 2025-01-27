namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>close-render-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>CLN CPA</c> part of the production.
/// </param>
internal sealed record CloseRenderBlockSyntax(Token Token) : CloseBlockSyntax(Token, TokenKind.CloseRenderBlock)
{
    public const String Production = "close-render-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
