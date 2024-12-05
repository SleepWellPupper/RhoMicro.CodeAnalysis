namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;
using System.Text;

abstract record JsonValueModel<T> : JsonValueModel
{
    public JsonValueModel(T value) => Value = value;
    public T Value { get; set; }
    public override Int32 GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);
    public virtual Boolean Equals(JsonValueModel<T>? other) => other is not null && EqualityComparer<T>.Default.Equals(Value, other.Value);
    public override String ToString() => base.ToString();
}

abstract record JsonValueModel
{
    public abstract void AppendTo(StringBuilder sb, CancellationToken ct);
    public override String ToString() => new StringBuilder().AppendModel(this, ct: default).ToString();
    public static JsonStringModel CreateString(String value) => new(value);
    public static JsonStringModel CreateString() => CreateString(String.Empty);
    public static JsonNumberModel CreateNumber(Double value) => new(value);
    public static JsonNumberModel CreateNumber() => CreateNumber(0d);
    public static JsonBooleanModel CreateBoolean(Boolean value) => new(value);
    public static JsonBooleanModel CreateBoolean() => CreateBoolean(false);
    public static JsonTypeModel CreateType(JsonType value) => new(value);
    public static JsonTypeModel CreateType() => CreateType(JsonType.Object);
    public static JsonObjectModel CreateObject() => CreateObject([]);
    public static JsonObjectModel CreateObject(Dictionary<String, JsonValueModel> value) => new(value);
    public static SimpleSchemaModel CreateSimpleSchema() => new();
    public static RefSchemaModel CreateRefSchema() => new();
    public static EnumSchemaModel CreateEnumSchema() => new();
    public static JsonArrayModel CreateArray() => CreateArray([]);
    public static JsonArrayModel CreateArray(List<JsonValueModel> value) => new(value);
    public static JsonSetModel CreateSet() => CreateSet([]);
    public static JsonSetModel CreateSet(HashSet<JsonValueModel> value) => new(value);
    public static JsonDynamicModel CreateDynamic() => new();
    public static JsonSchemaModel CreateSchema() => new();
}
