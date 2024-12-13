namespace RhoMicro.CodeAnalysis.Library.Text;
[IncludeFile]
internal interface IStringBuilder
{
    void Append(StringOrChar value);
    void AppendLine(StringOrChar value);
    void AppendLine();
}
