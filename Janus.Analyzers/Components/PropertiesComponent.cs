// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics.CodeAnalysis;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct PropertiesComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var properties = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var isNullable = m.Variants.Any(v => v.Type is { IsNullable: true } or { Kind: VariantTypeKind.Unknown });

            b.AppendLine(
                $$"""
                  {{Summary("Gets the variant of the union.")}}
                  public VariantModel Variant { get; } = VariantModel.Unknown;
                  {{Summary("Gets the value of the union.")}}
                  public object{{(isNullable ? "?" : String.Empty)}} Value 
                  {
                      get
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"return {new VariantAccessorComponent(v)};");
                          }, static (_, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append("throw CreateUnknownVariantException();");
                          })}}
                      }
                  }
                  """
            );

            foreach (var variant in m.Variants)
            {
                b.AppendLine(Summary(variant, static (v, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append(
                        $"Gets a value indicating whether the union is of the {C(v.Name)} variant, which is of type {Cref(v.Type.DocsId)}.");
                }))
                .SetCondition(variant.Type.Kind is VariantTypeKind.Reference)
                .AppendLine($"[{typeof(MemberNotNullWhenAttribute)}(true, nameof(As{variant.Name}))]")
                .UnsetCondition()
                .AppendLine(
                    $"""
                     public bool Is{variant.Name} => Variant.Kind is VariantKind.{variant.Name};
                     {Summary(variant, static (v, b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"Gets the unions value if the union is of the {C(v.Name)} variant; otherwise, {(v.Type.Kind is VariantTypeKind.Reference ? Langword("null") : Langword("default"))}.");
                     })}
                     {Remarks(variant, static (v, b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"""
                                   If the union is not of the {C(v.Name)} variant, no conversion is attempted.  
                                   In order to cast to a specific type, use {Cref("CastTo{TVariant}")} instead.
                                   """);
                     })}
                     public {variant.Type.NullableName}{(variant.Type is { Kind: VariantTypeKind.Reference, IsNullable: false } ? "?" : String.Empty)} As{variant.Name} =>  {new VariantAccessorComponent(variant, NullableCast: true)};
                     {Summary(variant, static (v, b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"Gets the represented value if the union is of the {C(v.Name)} variant; otherwise, an exception is thrown.");
                     })}
                     {Remarks(variant, static (v,b, ct) =>
                     {
                         ct.ThrowIfCancellationRequested();

                         b.Append($"""
                                   If the union is not of the {C(v.Name)} variant, no conversion is attempted.  
                                   In order to cast to a specific type, use {Cref("CastTo{TVariant}")} instead.
                                   """);
                     })}
                     public {variant.Type.NullableName} CastTo{variant.Name} =>  Variant.Kind is VariantKind.{variant.Name} ? {new VariantAccessorComponent(variant)} : throw CreateInvalidCastException(VariantKind.{variant.Name});
                     """);
            }
        });

        var region = Region("Properties", properties);

        builder.Append(region);
    }
}
