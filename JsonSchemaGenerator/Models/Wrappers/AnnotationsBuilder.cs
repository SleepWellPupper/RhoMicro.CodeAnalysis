namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

internal readonly record struct AnnotationsBuilder(JsonObjectModel Model)
{
    public JsonStringModel Description => Model.String("description");
    public JsonStringModel Title => Model.String("title");
    public void CopyTo(JsonObjectModel model)
    {
        if(Description.Value is { Length: > 0 } description)
            new AnnotationsBuilder(model).Description.Value = description;
        if(Title.Value is { Length: > 0 } title)
            new AnnotationsBuilder(model).Title.Value = title;
    }
}