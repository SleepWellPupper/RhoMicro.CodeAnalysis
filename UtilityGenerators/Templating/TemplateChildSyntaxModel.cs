namespace RhoMicro.CodeAnalysis.Templating;

internal abstract record TemplateChildSyntaxModel(SourceSpanModel Source, SyntaxKind Kind) : TemplateSyntaxBaseModel(Source, Kind)
{
    public override String ToString() => base.ToString();
}
