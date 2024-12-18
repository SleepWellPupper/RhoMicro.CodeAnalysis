namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;

internal readonly record struct PropertiesModel(JsonObjectModel Model)
{
    public JsonSchemaModel Add(String name) => Model.Schema(name);
}
