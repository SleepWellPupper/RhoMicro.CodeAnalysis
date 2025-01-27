namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>code-block</c> production.
/// </summary>
/// <param name="OpenCodeBlock">
/// Represents the <c>open-code-block</c> part of the production.
/// </param>
/// <param name="CodeBody">
/// Represents the <c>code-body</c> part of the production.
/// </param>
/// <param name="CloseCodeBlock">
/// Represents the <c>close-code-block</c> part of the production.
/// </param>
internal sealed record CodeBlockSyntax(OpenCodeBlockSyntax OpenCodeBlock, CodeBodySyntax CodeBody, CloseCodeBlockSyntax CloseCodeBlock) : BlockSyntax
{
    public const String Production = "code-block";
    public override void Accept<TVisitor>(TVisitor visitor)
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
