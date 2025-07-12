// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>render-block-head-syntax</c> production.
/// </summary>
/// <param name="OpenRenderBlock">
/// Represents the <c>open-render-block</c> part of the production.
/// </param>
/// <param name="Text">
/// Represents the <c>text</c> part of the production.
/// </param>
/// <param name="CloseRenderBlock">
/// Represents the <c>close-render-block</c> part of the production.
/// </param>
internal sealed record RenderBlockHeadSyntax(
    OpenRenderBlockSyntax OpenRenderBlock,
    TextSyntax Text,
    CloseRenderBlockSyntax CloseRenderBlock) : ISyntax
{
    public const String Production = "render-block-head";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
