namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Text;

internal sealed record JsonStringModel : JsonValueModel<String>
{
    public JsonStringModel(String value) : base(value) { }
    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append('"').Append(Value.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
    }

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonStringModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
