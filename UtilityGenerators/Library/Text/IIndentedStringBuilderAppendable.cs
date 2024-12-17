namespace RhoMicro.CodeAnalysis.Library.Text;

#if UTILITYGENERATORS
[IncludeFile]
#endif
internal interface IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder);
}
