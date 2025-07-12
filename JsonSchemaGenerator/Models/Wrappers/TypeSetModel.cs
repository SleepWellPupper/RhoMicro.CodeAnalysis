// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

internal readonly record struct TypeSetModel(JsonSetModel Model)
{
    public void Add(JsonType value) => Model.Add(value);
}
