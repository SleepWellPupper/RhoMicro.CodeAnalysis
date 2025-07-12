// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>trailing-trivia</c> production.
/// </summary>
/// <param name="Newline">
/// Represents the <c>newline</c> part of the production.
/// </param>
internal sealed record TrailingTriviaSyntax(NewlineSyntax Newline) : ISyntax
{
    public const String Production = "newline";
    public void Accept<TVisitor>(TVisitor visitor) where TVisitor : ISyntaxVisitor => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
