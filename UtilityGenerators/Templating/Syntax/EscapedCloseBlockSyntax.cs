// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>escaped-open-block</c> production.
/// </summary>
/// <param name="EscapeColon">
/// Represents the <c>escape-colon</c> part of the production.
/// </param>
/// <param name="CloseBlock">
/// Represents the <c>close-block</c> part of the production.
/// </param>
internal sealed record EscapedCloseBlockSyntax(EscapeColonSyntax EscapeColon, CloseBlockSyntax CloseBlock) : ISyntax
{
    public const String Production = "escaped-close-block";
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
