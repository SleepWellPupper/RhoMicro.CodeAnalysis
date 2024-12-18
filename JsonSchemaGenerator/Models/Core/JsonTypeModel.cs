namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Text;

internal sealed record JsonTypeModel : JsonValueModel<JsonType>
{
    public JsonTypeModel(JsonType value) : base(value) { }
    public static JsonTypeModel String => new(JsonType.String);
    public static JsonTypeModel Integer => new(JsonType.Integer);
    public static JsonTypeModel Boolean => new(JsonType.Boolean);
    public static JsonTypeModel Number => new(JsonType.Number);
    public static JsonTypeModel Object => new(JsonType.Object);
    public static JsonTypeModel Array => new(JsonType.Array);
    public static JsonTypeModel Null => new(JsonType.Null);
    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append(Value switch
        {
            JsonType.String => "\"string\"",
            JsonType.Integer => "\"integer\"",
            JsonType.Boolean => "\"boolean\"",
            JsonType.Number => "\"number\"",
            JsonType.Object => "\"object\"",
            JsonType.Array => "\"array\"",
            JsonType.Null => "\"null\"",
            _ => throw new InvalidOperationException("unknown json type")
        });
    }

    public override String ToString() => base.ToString();
    public Boolean Equals(JsonTypeModel? other) => base.Equals(other);
    public override Int32 GetHashCode() => base.GetHashCode();
}
