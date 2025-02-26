namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>empty-block</c> production.
/// </summary>
/// <param name="OpenBlock">
/// Represents the <c>open-block</c> part of the production.
/// </param>
/// <param name="CloseBlock">
/// Represents the <c>close-block</c> part of the production.
/// </param>
internal sealed record EmptyBlockSyntax(OpenBlockSyntax OpenBlock, CloseBlockSyntax CloseBlock) : ISyntax
{
    public const String Production = "empty-block";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
