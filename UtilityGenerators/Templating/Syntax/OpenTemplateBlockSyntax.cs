namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>open-template-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>OAB CLN</c> part of the production.
/// </param>
internal sealed record OpenTemplateBlockSyntax(Token Token) : OpenBlockSyntax(Token, TokenKind.OpenTemplateBlock)
{
    public const String Production = "open-template-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}