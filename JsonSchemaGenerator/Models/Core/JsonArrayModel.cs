namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;
using RhoMicro.CodeAnalysis.Library;

sealed record JsonArrayModel : JsonListLikeModel<List<JsonValueModel>>
{
    public JsonArrayModel(List<JsonValueModel> value) : base(value, new EquatableList<JsonValueModel>(value)) { }
    public override String ToString() => base.ToString();
}
