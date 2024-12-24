namespace RhoMicro.CodeAnalysis.Templating;

internal sealed record TextSyntaxModel(SourceSpanModel Source) : TextOrValueSyntaxModel(Source, SyntaxKind.Text)
{
    public override String ToString() => base.ToString();
}
