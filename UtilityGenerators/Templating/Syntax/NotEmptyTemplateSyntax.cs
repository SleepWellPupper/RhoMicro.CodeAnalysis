// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>not-empty-template</c> production.
/// </summary>
/// <param name="TemplateBlockBody">
/// Represents the <c>template-block-body</c> part of the production.
/// </param>
internal sealed record NotEmptyTemplateSyntax(TemplateBlockBodySyntax TemplateBlockBody) : ISyntax
{
    public const String Production = "not-empty-template";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
