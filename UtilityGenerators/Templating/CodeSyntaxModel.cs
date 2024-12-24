namespace RhoMicro.CodeAnalysis.Templating;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record CodeSyntaxModel(SourceSpanModel Source, EquatableList<TextOrValueSyntaxModel> Children) : TemplateChildSyntaxModel(Source, SyntaxKind.Code)
{
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
    public override String ToDebugString() => $"[{SyntaxKind} {String.Concat(Children.Select(c => $"{c.ToDebugString()}"))} ]";
#endif
    public override String ToString() => base.ToString();
}
