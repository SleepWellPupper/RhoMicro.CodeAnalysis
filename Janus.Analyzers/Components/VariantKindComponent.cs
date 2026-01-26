// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Library.Models.Collections;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;
using MembersListComponent =
    Lyra.ListComponent<Library.Models.Collections.EquatableList<UnionTypeAttribute.Model>, UnionTypeAttribute.Model,
        String>;

internal readonly record struct VariantKindComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = Model.HasDefaultVariant
            ? CreateMembersWithoutUnknown()
            : CreateMembersWithUnknown();

        var type = Create((members, Model), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (members, model) = t;

            b.Append($"""
                      {Summary(model, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"Enumerates the variants of {Cref(m.DocsCommentId)}.");
                      })}
                      {Type(
                          "public enum",
                          "VariantKind",
                          members,
                          baseTypeList: [EnumBackingType(model.Variants.Count + 1)])}
                      """);
        });

        var region = Region("VariantKind", type);

        builder.Append(region);
    }

    private MembersListComponent CreateMembersWithUnknown()
        => List(
            Model.Variants,
            static (v, i, _, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if (i is 0)
                {
                    b.AppendLine(
                        $"""
                         {Summary("Represents a not yet or incorrectly initialized union.")}
                         Unknown = 0,
                         """);
                }

                b.Append(
                    $"""
                     {Summary(v, static (v, b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"Represents the {C(v.Name)} variant.");
                     })}
                     {v.Name} = {i + 1}
                     """);
            },
            separator: ",\n");

    private MembersListComponent CreateMembersWithoutUnknown()
        => List(
            Model.Variants,
            static (v, i, _, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $"""
                     {Summary(v, static (v, b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"Represents the {C(v.Name)} variant.");
                     })}
                     {v.Name} = {i}
                     """);
            },
            separator: ",\n");
}
