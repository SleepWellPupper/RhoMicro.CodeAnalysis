// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct OperatorsComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var operators = ComponentFactory.Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            for (var i = 0; i < m.Variants.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var variant = m.Variants[i];

                if (variant.Type.IsInterface)
                {
                    continue;
                }

                if (i is not 0)
                {
                    b.AppendLine().AppendLine();
                }

                b.Append(
                    $"""
                     {Inheritdoc()}
                     public static implicit operator {new UnionTypeNameComponent(m)}({variant.Type.NullableName} value) => CreateFrom{variant.Name}(value);
                     {Inheritdoc()}
                     public static {(m.Variants.Count is 1 ? "implicit" : "explicit")} operator {variant.Type.NullableName}({new UnionTypeNameComponent(m)} union) => union.CastTo{variant.Name};   
                     """
                );
            }
        });

        var region = ComponentFactory.Region("Operators", operators);

        builder.Append(region);
    }
}
