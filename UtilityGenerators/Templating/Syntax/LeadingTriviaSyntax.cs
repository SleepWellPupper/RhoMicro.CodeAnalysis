// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>leading-trivia</c> production.
/// </summary>
/// <param name="Whitespaces">
/// Represents the <c>whitespaces</c> part of the production.
/// </param>
internal sealed record LeadingTriviaSyntax(WhitespacesSyntax Whitespaces) : ISyntax
{
    public const String Production = "leading-trivia";

    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
    public void Accept<TVisitor>(TVisitor visitor) where TVisitor : ISyntaxVisitor => visitor.Visit(this);
}