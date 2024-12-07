namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;
using System.Text;

abstract record JsonListLikeModel<TList> : JsonValueModel<TList>
    where TList : ICollection<JsonValueModel>, new()
{
    public JsonListLikeModel(TList value, Object equalityContract)
        : base(value) =>
        _equalityContract = equalityContract;

    private readonly Object _equalityContract;

    public override void AppendTo(StringBuilder sb, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _ = sb.Append('[');

        var i = 0;
        foreach(var item in Value)
        {
            ct.ThrowIfCancellationRequested();
            if(i > 0 && Value.Count > 1)
                _ = sb.Append(',');

            _ = sb.AppendModel(item, ct);
            i++;
        }

        _ = sb.Append(']');
    }

    public override String ToString() => base.ToString();
    public override Int32 GetHashCode() => _equalityContract.GetHashCode();
    public virtual Boolean Equals(JsonListLikeModel<TList>? other) => other is not null && _equalityContract.Equals(other._equalityContract);

    private TModel Add<TModel>(TModel model)
        where TModel : JsonValueModel
    {
        Value.Add(model);
        return model;
    }

    public JsonStringModel Add(String value) => Add(new JsonStringModel(value));
    public JsonNumberModel Add(Number value) => Add(new JsonNumberModel(value));
    public JsonBooleanModel Add(Boolean value) => Add(new JsonBooleanModel(value));
    public JsonTypeModel Add(JsonType value) => Add(new JsonTypeModel(value));
    public JsonObjectModel AddObject() => Add(new JsonObjectModel());
}
