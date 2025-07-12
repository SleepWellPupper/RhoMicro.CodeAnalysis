// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>code-block-body</c> production.
/// </summary>
/// <param name="Children">
/// Represents the <c>*code-block-body-child</c> part of the production.
/// </param>
internal sealed record CodeBlockBodySyntax(EquatableList<CodeBlockBodyChildSyntax> Children) : ISyntax
{
    public const String Production = "code-block-body";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
