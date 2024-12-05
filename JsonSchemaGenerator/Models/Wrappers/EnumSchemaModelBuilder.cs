namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;

readonly record struct EnumSchemaModelBuilder(EnumSchemaModel Model)
{
    public void Add(String value) => Model.Array("enum").Add(value);
    public void Add(Double value) => Model.Array("enum").Add(value);
    public void Add(Boolean value) => Model.Array("enum").Add(value);
}
