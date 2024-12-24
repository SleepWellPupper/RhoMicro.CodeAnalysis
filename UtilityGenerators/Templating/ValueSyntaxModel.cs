namespace RhoMicro.CodeAnalysis.Templating;

internal sealed record ValueSyntaxModel(SourceSpanModel Source) : TextOrValueSyntaxModel(Source, SyntaxKind.Value)
{
    public override String ToString() => base.ToString();
}
