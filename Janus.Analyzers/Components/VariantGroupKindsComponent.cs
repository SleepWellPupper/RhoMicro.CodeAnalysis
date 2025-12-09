// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Runtime.CompilerServices;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct VariantGroupKindsComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = List(
            Model.VariantGroups,
            static (g, i, _, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.AppendLine(Summary(g, static (g, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"Represents the <c>{g}</c> group.");
                }));

                if (i is 0)
                {
                    b.Append($"{g} = 0");
                }
                else
                {
                    b.Append($"{g} = 1 << {i - 1}");
                }
            },
            separator: ",\n");
        var type = Create((Model, members), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, members) = t;

            b.Append(
                $"""
                 {Summary(model, static (m, b, ct) =>
                 {
                     ct.ThrowIfCancellationRequested();

                     b.Append($"Enumerates the kinds of groups variants of the {Cref(m.DocsCommentId)} may be a part of.");
                 })}
                 {Type(
                     "public enum",
                     "VariantGroupKinds",
                     members,
                     attributes:
                     [
                         Attribute(TypeName<FlagsAttribute>(), arguments: [])
                     ],
                     baseTypeList: [FlagsEnumBackingType(model.VariantGroups.Count - 1)])}
                 """
            );
        });
        var region = Region("VariantGroupKinds", type);
        builder.Append(region);
    }
}
