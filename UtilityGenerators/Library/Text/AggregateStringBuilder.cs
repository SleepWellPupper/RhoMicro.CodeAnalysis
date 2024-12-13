namespace RhoMicro.CodeAnalysis.Library.Text;

[NonEquatable]
[IncludeFile]
internal sealed partial class AggregateStringBuilder(IEnumerable<IStringBuilder> builders, CancellationToken ct) : IStringBuilder
{
    public void Append(StringOrChar value)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var builder in builders)
        {
            ct.ThrowIfCancellationRequested();

            builder.Append(value);
        }
    }
    public void AppendLine(StringOrChar value)
    {
        ct.ThrowIfCancellationRequested();

        foreach(var builder in builders)
        {
            ct.ThrowIfCancellationRequested();

            builder.AppendLine(value);
        }
    }
    public void AppendLine()
    {
        ct.ThrowIfCancellationRequested();

        foreach(var builder in builders)
        {
            ct.ThrowIfCancellationRequested();

            builder.AppendLine();
        }
    }
}
