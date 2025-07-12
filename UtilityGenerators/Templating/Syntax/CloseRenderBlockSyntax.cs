// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>close-render-block</c> production.
/// </summary>
internal sealed record CloseRenderBlockSyntax : ISyntax
{
    public const String Production = "close-render-block";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="token">
    /// The <c>CLN CPA</c> part of the production.
    /// </param>
    public CloseRenderBlockSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.CloseRenderBlock);

        Token = token;
    }

    /// <summary>
    /// Gets the <c>CLN CPA</c> part of the production.
    /// </summary>
    public Token Token { get; }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}

