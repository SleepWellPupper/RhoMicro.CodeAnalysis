// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using System;

internal readonly record struct EnumSchemaModelBuilder(EnumSchemaModel Model)
{
    public void Add(String value) => Model.Array("enum").Add(value);
    public void Add(Number value) => Model.Array("enum").Add(value);
    public void Add(Boolean value) => Model.Array("enum").Add(value);
}
