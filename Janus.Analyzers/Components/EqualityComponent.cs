// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct EqualityComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = ComponentFactory.Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append(
                $$"""
                  {{Inheritdoc()}}
                  public override bool Equals(object? obj) => obj is {{new UnionTypeNameComponent(m)}} other && Equals(other);
                  """
            );

            if (!m.IsEqualsUserProvided)
            {
                b.Append(
                    $$"""
                          
                      {{Inheritdoc()}}
                      public bool Equals({{new UnionTypeNameComponent(m, RenderNullable: true)}} other)
                      {
                          {{ComponentFactory.Create(m, static (m, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              if (m.TypeKind is not UnionTypeKind.Class)
                              {
                                  return;
                              }

                              b.Append(
                                  $$"""
                                    if(other is null)
                                    {
                                        return false;
                                    }

                                    if(object.ReferenceEquals(this, other))
                                    {
                                        return true;
                                    }
                                    
                                    
                                    """
                              );
                          })}}if(Variant != other.Variant)
                          {
                              return false;
                          }
                          
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"return global::System.Collections.Generic.EqualityComparer<{v.Type.NullableName}>.Default.Equals({new VariantAccessorComponent(v)}, {new VariantAccessorComponent(v, InstanceExpression: "other")});");
                          })}}
                      }
                      """
                );
            }

            if (!m.IsGetHashCodeUserProvided)
            {
                b.Append($$"""

                           {{Inheritdoc()}}
                           public override int GetHashCode()
                           {
                               {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                               {
                                   ct.ThrowIfCancellationRequested();

                                   b.Append($"return {typeof(HashCode)}.Combine(Variant, {new VariantAccessorComponent(v)});");
                               })}}
                           }
                           """
                );
            }

            b.Append(
                $"""

                 {new EqualityOperatorComponent(m)}

                 {new ToStringComponent(m)}
                 """
            );
        });

        var region = Region("Equality & ToString", methods);

        builder.Append(region);
    }
}
