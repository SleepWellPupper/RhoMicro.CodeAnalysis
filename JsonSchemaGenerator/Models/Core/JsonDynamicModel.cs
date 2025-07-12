// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;
using System.Text;

internal sealed record JsonDynamicModel : JsonValueModel<JsonValueModel>
{
    public JsonDynamicModel() : base(CreateArray()) { }
    public override void AppendTo(StringBuilder sb, CancellationToken ct) => Value.AppendTo(sb, ct);
    public JsonArrayModel Array => Value is JsonArrayModel a
        ? a
        : (JsonArrayModel)( Value = CreateArray() );
    public JsonSchemaModel Schema => Value is JsonSchemaModel a
        ? a
        : (JsonSchemaModel)( Value = CreateSchema() );
    public JsonObjectModel Object => Value is JsonObjectModel a
        ? a
        : (JsonObjectModel)( Value = CreateObject() );
    public JsonNumberModel Number => Value is JsonNumberModel a
        ? a
        : (JsonNumberModel)( Value = CreateNumber() );
    public JsonStringModel String => Value is JsonStringModel a
        ? a
        : (JsonStringModel)( Value = CreateString() );
    public JsonBooleanModel Boolean => Value is JsonBooleanModel a
        ? a
        : (JsonBooleanModel)( Value = CreateBoolean() );

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonDynamicModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
