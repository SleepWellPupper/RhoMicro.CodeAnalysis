// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics.CodeAnalysis;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct MappingComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append(
                $$"""
                  {{Summary("Maps this union to another.")}}
                  {{Param("factory", "The factory used to create the target union instance.")}}
                  {{TypeParam("TUnion", "The type of union to map to.")}}
                  {{TypeParam("TFactory", "The type of factory used to perform the mapping operation.")}}
                  {{Returns("The created union instance.")}}
                  public TUnion MapTo<TUnion, TFactory>(
                      TFactory factory)
                      where TFactory : global::RhoMicro.CodeAnalysis.IUnionFactory<TUnion>
                  {
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"return factory.Create({new VariantAccessorComponent(v)});");
                      })}}
                  }

                  {{Summary("Attempts to map this union to another.")}}
                  {{Param("factory", "The factory used to create the target union instance.")}}
                  {{Param("union", static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"The union instance if one could be created; otherwise, {Langword("default")}.");
                  })}}
                  {{TypeParam("TUnion", "The type of union to map to.")}}
                  {{TypeParam("TFactory", "The type of factory used to perform the mapping operation.")}}
                  {{Returns(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"{Langword("true")} if a union instance could be created; otherwise, {Langword("false")}.");
                  })}}
                  public bool TryMapTo<TUnion, TFactory>(
                      TFactory factory,
                      [{{typeof(NotNullWhenAttribute)}}(true)]
                      out TUnion? union)
                      where TFactory : global::RhoMicro.CodeAnalysis.IUnionFactory<TUnion>
                  {
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"return factory.TryCreate({new VariantAccessorComponent(v)}, out union);");
                      })}}
                  }
                  """
            );
        });

        var region = Region("Mapping", methods);

        builder.Append(region);
    }
}
