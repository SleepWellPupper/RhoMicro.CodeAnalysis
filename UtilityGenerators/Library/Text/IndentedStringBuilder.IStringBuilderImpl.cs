namespace RhoMicro.CodeAnalysis.Library.Text;

[IncludeFile]
#pragma warning disable IDE0040 // Add accessibility modifiers
partial class IndentedStringBuilder : IStringBuilder
#pragma warning restore IDE0040 // Add accessibility modifiers
{
    void IStringBuilder.Append(StringOrChar value) => Append(value);
    void IStringBuilder.AppendLine(StringOrChar value) => AppendLine(value);
    void IStringBuilder.AppendLine() => AppendLine();
}