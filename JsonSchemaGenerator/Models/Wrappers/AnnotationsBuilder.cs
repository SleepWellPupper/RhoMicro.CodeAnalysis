namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

readonly record struct AnnotationsBuilder(JsonObjectModel Model)
{
    public JsonStringModel Description => Model.String("description");
    public JsonStringModel Title => Model.String("title");
}