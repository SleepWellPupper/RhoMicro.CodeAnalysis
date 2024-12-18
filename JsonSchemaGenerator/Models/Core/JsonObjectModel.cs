namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;
using System.Text;

using RhoMicro.CodeAnalysis.Library;

internal record JsonObjectModel : JsonValueModel<Dictionary<String, JsonValueModel>>
{
    public JsonObjectModel(Dictionary<String, JsonValueModel> value) : base(value) => _properties = new(Value);
    public JsonObjectModel() : this([]) { }
    private readonly EquatableDictionary<String, JsonValueModel> _properties;
    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append('{');

        var i = 0;
        foreach(var (key, value) in Value)
        {
            ct.ThrowIfCancellationRequested();
            if(i > 0 && Value.Count > 1)
                _ = sb.Append(',');

            _ = sb.Append('"').Append(key).Append("\":").AppendModel(value, ct);
            i++;
        }

        _ = sb.Append('}');
    }

    public override String ToString() => base.ToString();
    public override Int32 GetHashCode() => _properties.GetHashCode();
    public virtual Boolean Equals(JsonObjectModel? other) => other is not null && _properties.Equals(other._properties);

    public void UnsetProperty(String name) => Value.Remove(name);
    public void SetProperty(String name, JsonValueModel value) => Value[name] = value;
    private TModel GetOrSet<TModel, TValue>(String name, Func<TValue, TModel> factory, TValue value)
        where TModel : JsonValueModel
    {
        if(!Value.TryGetValue(name, out var v) || v is not TModel result)
        {
            result = factory.Invoke(value);
            Value.Add(name, result);
        }

        return result;
    }
    private TModel GetOrSet<TModel>(String name, Func<TModel> factory)
        where TModel : JsonValueModel
    {
        if(!Value.TryGetValue(name, out var v) || v is not TModel result)
        {
            result = factory.Invoke();
            Value.Add(name, result);
        }

        return result;
    }
    public JsonStringModel String(String name) => GetOrSet(name, CreateString);
    public JsonTypeModel Type(String name) => GetOrSet(name, CreateType);
    public JsonArrayModel Array(String name) => GetOrSet(name, CreateArray);
    public JsonBooleanModel Boolean(String name) => GetOrSet(name, CreateBoolean);
    public JsonSetModel Set(String name) => GetOrSet(name, CreateSet);
    public JsonObjectModel Object(String name) => GetOrSet(name, CreateObject);
    public JsonDynamicModel Dynamic(String name) => GetOrSet(name, CreateDynamic);
    public JsonDynamicModel Dynamic(String name, JsonValueModel defaultValue) =>
        GetOrSet(name, () =>
        {
            var result = CreateDynamic();
            result.Value = defaultValue;
            return result;
        });
    public JsonSchemaModel Schema(String name) => GetOrSet(name, CreateSchema);
}
