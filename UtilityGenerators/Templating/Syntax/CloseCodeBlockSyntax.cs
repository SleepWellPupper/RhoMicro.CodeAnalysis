namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>close-code-block</c> production.
/// </summary>
/// <param name="Token">
/// Represents the <c>CLN CCB</c> part of the production.
/// </param>
internal sealed record CloseCodeBlockSyntax(Token Token) : CloseBlockSyntax(Token, TokenKind.CloseCodeBlock)
{
    public const String Production = "close-code-block";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
