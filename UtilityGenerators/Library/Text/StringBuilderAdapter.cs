namespace RhoMicro.CodeAnalysis.Library.Text;
using System;
using System.Text;

[NonEquatable]
[IncludeFile]
internal sealed partial class CancellableStringBuilder(StringBuilder sb, CancellationToken ct) : IStringBuilder
{
    public CancellableStringBuilder(CancellationToken ct) : this(new(), ct) { }
    public void Append(StringOrChar value)
    {
        ct.ThrowIfCancellationRequested();
        _ = value.IsString
            ? sb.Append((String)value)
            : sb.Append((Char)value);
    }
    public void AppendLine(StringOrChar value)
    {
        ct.ThrowIfCancellationRequested();
        _ = value.IsString
            ? sb.AppendLine((String)value)
            : sb.Append((Char)value).AppendLine();
    }
    public void AppendLine()
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.AppendLine();
    }
    public override String ToString()
    {
        ct.ThrowIfCancellationRequested();

        var result = sb.ToString();

        return result;
    }
}