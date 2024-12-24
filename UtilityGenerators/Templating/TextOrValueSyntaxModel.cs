namespace RhoMicro.CodeAnalysis.Templating;

internal abstract record TextOrValueSyntaxModel(SourceSpanModel Source, SyntaxKind Kind) : TemplateChildSyntaxModel(Source, Kind)
{
    public override String ToString() => base.ToString();
}
