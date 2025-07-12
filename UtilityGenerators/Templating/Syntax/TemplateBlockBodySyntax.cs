// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>template-block-body</c> production.
/// </summary>
/// <param name="Children">
/// Represents the <c>*template-block-body-child</c> part of the production.
/// </param>
internal sealed record TemplateBlockBodySyntax(EquatableList<TemplateBlockBodyChildSyntax> Children) : ISyntax
{
    public const String Production = "template-block-body";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}