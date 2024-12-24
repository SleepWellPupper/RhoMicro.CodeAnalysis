namespace RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal interface IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder);
}
