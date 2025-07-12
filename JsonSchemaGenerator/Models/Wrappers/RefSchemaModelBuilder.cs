// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

internal readonly record struct RefSchemaModelBuilder(RefSchemaModel Model)
{
    public void Ref(Id id, Id to) => Model.String("$ref").Value = id.Relative(to);
}
