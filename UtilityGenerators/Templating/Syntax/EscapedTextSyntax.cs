namespace RhoMicro.CodeAnalysis.Templating.Syntax;
/// <summary>
/// Represents the <c>escaped-text</c> production.
/// </summary>
internal abstract record EscapedTextSyntax : TextChildSyntax
{
    public override String ToString() => this.ToAstString();
}
