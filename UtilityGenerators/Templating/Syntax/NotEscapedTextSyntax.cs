namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>not-escaped-text</c> production.
/// </summary>
internal sealed record NotEscapedTextSyntax : TextChildSyntax
{
    public NotEscapedTextSyntax(Token token)
    {
        ThrowHelpers.ThrowIfKindNotEqual(token, TokenKind.Text);
        Token = token;
    }

    public const String Production = "not-escaped-text";

    /// <summary>
    /// Gets the matched token.
    /// </summary>
    public Token Token { get; }

    public override void Accept<TVisitor>(TVisitor visitor)
        => visitor.Visit(this);
    public override String ToString() => this.ToAstString();
}