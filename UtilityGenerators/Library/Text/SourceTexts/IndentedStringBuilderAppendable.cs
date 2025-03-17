namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
#if GENERATOR
[NonEquatable]
#endif
#if SOURCETEXTS_LIBRARY
public
#else
internal
#endif
 sealed partial class IndentedStringBuilderAppendable(Action<IndentedStringBuilder> strategy) : IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder) => strategy.Invoke(builder);
}
