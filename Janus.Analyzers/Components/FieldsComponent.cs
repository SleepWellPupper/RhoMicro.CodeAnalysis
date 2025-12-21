// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct FieldsComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fields = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var unmanagedDefined = false;
            var referenceDefined = false;

            for (var i = 0; i < m.Variants.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var variant = m.Variants[i];

                switch (variant.Type.Kind)
                {
                    case VariantTypeKind.Unmanaged:
                        if (unmanagedDefined)
                        {
                            continue;
                        }

                        if (i is not 0)
                        {
                            b.AppendLine();
                        }

                        b.Append(
                            $"""
                             {Summary("The container used to store values for unmanaged variants of the union.")}
                             private readonly UnmanagedVariantsContainer _unmanagedVariantsContainer = default;
                             """);
                        unmanagedDefined = true;
                        break;
                    case VariantTypeKind.Value or VariantTypeKind.Unknown:
                        if (i is not 0)
                        {
                            b.AppendLine();
                        }

                        b.Append($"""
                                  {Summary(variant, static (v, b, ct) =>
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      b.Append($"The value when representing the {C(v.Name)} variant.");
                                  })}
                                  private readonly {variant.Type.NullableName} _{variant.Name} = default;
                                  """);
                        break;
                    case VariantTypeKind.Reference:
                        if (referenceDefined)
                        {
                            continue;
                        }

                        if (i is not 0)
                        {
                            b.AppendLine();
                        }

                        b.Append($"""
                                  {Summary("The container used to store values for reference type variants.")}
                                  private readonly object? _referenceVariantsContainer = default;
                                  """);
                        referenceDefined = true;
                        break;
                }
            }
        });

        var region = Region("Fields", fields);

        builder.Append(region);
    }
}
