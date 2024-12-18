namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System.Text;

internal sealed record JsonSchemaModel : JsonValueModel
{
    private readonly List<SubSchemaModelBuilder> _subSchemata = [];
    private JsonObjectModel? _result;
    private Int32 _buildLock;
    public SubSchemaModelBuilder SubSchema()
    {
        var result = new SubSchemaModelBuilder();
        _subSchemata.Add(result);
        return result;
    }
    public Int32 Size => _subSchemata.Count;
    public JsonObjectModel Build(Boolean includeId = false, CancellationToken ct = default)
    {
        if(_result is not null)
            return _result;

        while(Interlocked.CompareExchange(ref _buildLock, 1, 0) == 1)
            ct.ThrowIfCancellationRequested();

        try
        {
            if(_result is not null)
                return _result;

            _result = BuildCore(includeId, ct);
            return _result;
        } finally
        {
            _buildLock = 0;
        }
    }

    private JsonObjectModel BuildCore(Boolean includeId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var builtSchemata = new List<JsonValueModel>();
        foreach(var subSchema in _subSchemata)
        {
            ct.ThrowIfCancellationRequested();
            subSchema.Build(builtSchemata, includeId, ct);
        }

        if(builtSchemata is [JsonObjectModel { } singleSchema])
        {
            if(singleSchema is SimpleSchemaModel)
            {
                var type = singleSchema.Set("type");
                if(type.Value.Count == 0)
                {
                    _ = type.Add(JsonType.Object);
                    singleSchema.Boolean("additionalProperties").Value = false;
                }
            }

            return singleSchema;
        } else
        {
            ct.ThrowIfCancellationRequested();
            var anyOf = new JsonObjectModel();
            var hasSimple = false;
            foreach(var builtSchema in builtSchemata)
            {
                ct.ThrowIfCancellationRequested();
                anyOf.Array("anyOf").Value.Add(builtSchema);
                //hasSimple |= builtSchema is SimpleSchemaModel;
            }

            if(!hasSimple)
                return anyOf;

            var result = new JsonObjectModel();
            result.Array("anyOf").Value.Add(anyOf);

            //var type = new JsonObjectModel();
            //_ = type.Set("type").Add(JsonType.Object);
            //result.Array("allOf").Value.Add(type);

            //result.Boolean("additionalProperties").Value = false;

            return result;
        }
    }

    public override void AppendTo(StringBuilder sb, CancellationToken ct) => Build(includeId: false, ct).AppendTo(sb, ct);
    public override Int32 GetHashCode() => Build().GetHashCode();
    public Boolean Equals(JsonSchemaModel? other) => other is not null && Build().Equals(other.Build());
    public override String ToString() => base.ToString();
}
