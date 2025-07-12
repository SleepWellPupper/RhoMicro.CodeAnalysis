// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>not-escaped-text</c> production.
/// </summary>
/// <param name="Children">
/// Represents the <c>not-escaped-text-child *not-escaped-text-child</c> part of the production.
/// </param>
internal sealed record NotEscapedTextSyntax(EquatableList<NotEscapedTextChildSyntax> Children) : ISyntax
{
    public const String Production = "not-escaped-text";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
