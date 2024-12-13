namespace RhoMicro.CodeAnalysis.Library.Text;

[IncludeFile]
internal interface IIndentedStringBuilderAppendable
{
    public void AppendTo(IndentedStringBuilder builder);
}
