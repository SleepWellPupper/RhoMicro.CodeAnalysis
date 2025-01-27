namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>trivia-block</c> production.
/// </summary>
/// <param name="Trivia">
/// Represents the <c>trivia</c> part of the production
/// </param>
/// <param name="Block">
/// Represents the <c>block</c> part of the production
/// </param>
internal sealed record TriviaBlockSyntax(TriviaSyntax Trivia, BlockSyntax Block) : ISyntax
{
    public const String Production = "trivia-block";
    public override String ToString() => this.ToAstString();
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
}
