namespace RhoMicro.CodeAnalysis.Library.Text;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
internal sealed partial class IndentedStringBuilderAppendable(Action<IndentedStringBuilder> strategy) : IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder) => strategy.Invoke(builder);
}