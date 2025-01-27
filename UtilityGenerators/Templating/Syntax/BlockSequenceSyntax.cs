namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

/// <summary>
/// Represents the <c>block-sequence</c> production.
/// </summary>
/// <param name="Block">
/// Represents the <c>block</c> part of the production.
/// </param>
/// <param name="Blocks">
/// Represents the <c>*newline-block</c> part of the production.
/// </param>
internal sealed record BlockSequenceSyntax(BlockSyntax Block, EquatableList<TriviaBlockSyntax> Blocks) : TemplateBlockBodyChildSyntax
{
    /// <summary>
    /// Represents the <c>block-sequence</c> production.
    /// </summary>
    /// <param name="Block">
    /// Represents the <c>block</c> part of the production.
    /// </param>
#pragma warning disable IDE1006 // Naming Styles
    public BlockSequenceSyntax(BlockSyntax Block) : this(Block, []) { }
#pragma warning restore IDE1006 // Naming Styles
    public const String Production = "block-sequence";
    public override void Accept<TVisitor>(TVisitor visitor) => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}
