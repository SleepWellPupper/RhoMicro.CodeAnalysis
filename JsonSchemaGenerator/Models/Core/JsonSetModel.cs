// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Models;

using RhoMicro.CodeAnalysis.Library;

internal sealed record JsonSetModel : JsonListLikeModel<HashSet<JsonValueModel>>
{
    public JsonSetModel(HashSet<JsonValueModel> value)
        : base(value, new EquatableSet<JsonValueModel>(new ImmutableHashSetAdapter<JsonValueModel>(value)))
    { }
    public override String ToString() => base.ToString();
}
