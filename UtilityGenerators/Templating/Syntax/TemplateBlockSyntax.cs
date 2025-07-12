// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>template-block</c> production.
/// </summary>
/// <param name="LeadingTrivia">
/// Represents the <c>[leading-trivia]</c> part of the production.
/// </param>
/// <param name="OpenTemplateBlock">
/// Represents the <c>open-template-block</c> part of the production.
/// </param>
/// <param name="TemplateBlockBody">
/// Represents the <c>template-block-body</c> part of the production.
/// </param>
/// <param name="CloseTemplateBlock">
/// Represents the <c>close-template-block</c> part of the production.
/// </param>
/// <param name="TrailingTrivia">
/// Represents the <c>[trailing-trivia]</c> part of the production.
/// </param>
internal sealed record TemplateBlockSyntax(
    LeadingTriviaSyntax? LeadingTrivia,
    OpenTemplateBlockSyntax OpenTemplateBlock,
    TemplateBlockBodySyntax TemplateBlockBody,
    CloseTemplateBlockSyntax CloseTemplateBlock,
    TrailingTriviaSyntax? TrailingTrivia) : ISyntax
{
    public const String Production = "template-block";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
