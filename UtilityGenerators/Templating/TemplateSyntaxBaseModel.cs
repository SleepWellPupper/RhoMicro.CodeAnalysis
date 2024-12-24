namespace RhoMicro.CodeAnalysis.Templating;

using System.Diagnostics;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[DebuggerDisplay("{ToDebugString()}")]
#endif
internal abstract record TemplateSyntaxBaseModel(SourceSpanModel Source, SyntaxKind SyntaxKind)
{
    public override String ToString() => $"{SyntaxKind.ToStringFast()} {Source.Start}+{Source.Length} \"{Source}\"";
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
    public virtual String ToDebugString() => $"[{SyntaxKind} {Source} ]";
#endif
}
