// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct EqualityOperatorComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Model is
            {
                Settings.EqualityOperatorsSetting: EqualityOperatorsSetting.OmitOperators
            } or
            {
                Settings.EqualityOperatorsSetting: EqualityOperatorsSetting.EmitOperatorsIfValueType,
                TypeKind: not UnionTypeKind.Struct
            })
        {
            return;
        }

        builder.Append($$"""
                         {{Inheritdoc()}}
                         public static bool operator ==({{new UnionTypeNameComponent(Model)}} x, {{new UnionTypeNameComponent(Model)}} y) => 
                             global::System.Collections.Generic.EqualityComparer<{{new UnionTypeNameComponent(Model)}}>.Default.Equals(x, y);
                         {{Inheritdoc()}}
                         public static bool operator !=({{new UnionTypeNameComponent(Model)}} x, {{new UnionTypeNameComponent(Model)}} y) =>
                             !(x == y);
                         """
        );
    }
}
