// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Text;

internal sealed record JsonBooleanModel : JsonValueModel<Boolean>
{
    public JsonBooleanModel(Boolean value) : base(value) { }
    public static JsonBooleanModel False => new(false);
    public static JsonBooleanModel True => new(true);
    public static JsonBooleanModel Create(Boolean value) => new(value);
    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append(Value ? "true" : "false");
    }

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonBooleanModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
