namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>open-code-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>OCB CLN</c> part of the production.
/// </param>
internal sealed record OpenCodeBlockSyntax(Token Token) : OpenBlockSyntax(Token, TokenKind.OpenCodeBlock)
{
    public const String Production = "open-code-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
