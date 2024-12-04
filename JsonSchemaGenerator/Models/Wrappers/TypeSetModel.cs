namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

readonly record struct TypeSetModel(JsonSetModel Model)
{
    public void Add(JsonType value) => Model.Add(value);
}
