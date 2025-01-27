namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>open-render-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>OPA CLN</c> part of the production.
/// </param>
internal sealed record OpenRenderBlockSyntax(Token Token) : OpenBlockSyntax(Token, TokenKind.OpenRenderBlock)
{
    public const String Production = "open-render-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
