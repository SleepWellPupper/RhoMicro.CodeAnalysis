// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;

internal record TypeNames
{
    public TypeNames(UnionModel model)
    {
        IUnionFactory = ComponentFactory.TypeName($"global::RhoMicro.CodeAnalysis.IUnionFactory<{new UnionTypeNameComponent(model)}>");
    }

    public TypeNameComponent IUnion { get; } = ComponentFactory.TypeName("global::RhoMicro.CodeAnalysis.IUnion");
    public TypeNameComponent IUnionFactory { get; }
}
