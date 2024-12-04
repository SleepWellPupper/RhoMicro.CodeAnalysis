namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Globalization;
using System.Text;

sealed record JsonNumberModel : JsonValueModel<Double>
{
    public JsonNumberModel(Double value) : base(value) { }
    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append(Value.ToString("0.#", CultureInfo.InvariantCulture));
    }

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonNumberModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
