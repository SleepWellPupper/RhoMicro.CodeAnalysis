namespace RhoMicro.CodeAnalysis.Templating.Syntax;

using RhoMicro.CodeAnalysis.Templating;
using RhoMicro.CodeAnalysis.Templating.Syntax.Visitors;

/// <summary>
/// Represents the <c>close-template-block</c> production.
/// </summary>
internal sealed record CloseTemplateBlockSyntax : ISyntax
{
    public const String Production = "close-template-block";

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="token">
    /// The <c>CLN CAB</c> part of the production.
    /// </param>
    public CloseTemplateBlockSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.CloseTemplateBlock);

        Token = token;
    }

    /// <summary>
    /// Gets the <c>CLN CAB</c> part of the production.
    /// </summary>
    public Token Token { get; }

    public void Accept<TVisitor>(TVisitor visitor)
        where TVisitor : ISyntaxVisitor
        => visitor.Visit(this);
    public override String ToString() => this.ToXmlTreeString(CancellationToken.None);
}