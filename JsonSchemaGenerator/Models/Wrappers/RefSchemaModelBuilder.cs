namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

readonly record struct RefSchemaModelBuilder(RefSchemaModel Model)
{
    public void Ref(String id) => Model.String("$ref").Value = $"./{id}.json";
}
