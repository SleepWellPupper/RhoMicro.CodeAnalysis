// SPDX-License-Identifier: MPL-2.0

// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>escape-colon</c> production.
/// </summary>
internal sealed record EscapeColonSyntax : ISyntax
{
    public EscapeColonSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.EscapeColon);
        Token = token;
    }
    public const String Production = "escape-colon";
    /// <summary>
    /// Gets the matched token.
    /// </summary>
    public Token Token { get; }
    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}
