namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;

readonly record struct StringArrayModel(JsonArrayModel Model)
{
    public void Add(String value) => Model.Add(value);
}
