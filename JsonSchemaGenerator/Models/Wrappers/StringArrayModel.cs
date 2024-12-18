namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;

internal readonly record struct StringArrayModel(JsonArrayModel Model)
{
    public void Add(String value) => Model.Add(value);
}
