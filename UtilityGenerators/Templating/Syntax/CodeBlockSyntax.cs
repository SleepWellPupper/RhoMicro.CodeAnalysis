// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>code-block</c> production.
/// </summary>
/// <param name="LeadingTrivia">
/// Represents the <c>[leading-trivia]</c> part of the production.
/// </param>
/// <param name="OpenCodeBlock">
/// Represents the <c>open-code-block</c> part of the production.
/// </param>
/// <param name="CodeBody">
/// Represents the <c>code-block-body</c> part of the production.
/// </param>
/// <param name="CloseCodeBlock">
/// Represents the <c>close-code-block</c> part of the production.
/// </param>
/// <param name="TrailingTrivia">
/// Represents the <c>[trailing-trivia]</c> part of the production.
/// </param>
internal sealed record CodeBlockSyntax(
    LeadingTriviaSyntax? LeadingTrivia,
    OpenCodeBlockSyntax OpenCodeBlock,
    CodeBlockBodySyntax CodeBody,
    CloseCodeBlockSyntax CloseCodeBlock,
    TrailingTriviaSyntax? TrailingTrivia) : ISyntax
{
    public const String Production = "code-block";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
