// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct InspectionsComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            // TODO:
            // when mayBeNull in Is<T>(out u):
            // comment explaining that value is guaranteed to be null if returning false, even without annotation

            var mayBeNull = m.Variants.Any(static v =>
                v.Type is { IsNullable: true } or { Kind: VariantTypeKind.Unknown });

            b.Append(
                $$"""
                  {{Summary("""
                            Converts the value contained by the union to the provided type. If the variant of the union 
                            is convertible to the provided type, a conversion will be performed; otherwise, an exception 
                            is thrown.
                            """)}}
                  {{TypeParam("TVariant", "The type to convert the unions value to.")}}
                  {{Returns("The converted value.")}}
                  public TVariant{{(mayBeNull ? "?" : String.Empty)}} CastTo<TVariant>() 
                  {
                      {{new VariantsSwitchComponent(m, static (v, m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          var annotation = v.Type is { IsNullable: true } or { Kind: VariantTypeKind.Unknown } ? "?" : String.Empty;

                          b.Append(
                              $"""
                               // The jit will optimize this conversion and prevent boxing.
                               var result = (TVariant{annotation})(object{annotation}){new VariantAccessorComponent(v)};

                               return result;
                               """
                          );
                      })}}
                  } 

                  {{Summary("Attempts to convert the value contained by the union to the provided type.")}}
                  {{TypeParam("TVariant", "The type to convert the unions value to.")}}
                  {{Returns(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"The converted value if the conversion is possible; otherwise, {Langword("default")}.");
                  })}}
                  public TVariant? As<TVariant>()
                  {
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          var annotation = v.Type is { IsNullable: true } or { Kind: VariantTypeKind.Unknown } ? "?" : String.Empty;

                          b.Append(
                              $$"""
                                if(typeof(TVariant) == typeof({{v.Type.NullableValueTypeName}}) || typeof(TVariant).IsAssignableFrom(typeof({{v.Type.NullableValueTypeName}})))
                                {
                                    // The jit will optimize this conversion and prevent boxing.
                                    var result = (TVariant{{annotation}})(object{{annotation}}){{new VariantAccessorComponent(v)}};

                                    return result;
                                }
                                else
                                {
                                    return default;
                                }
                                """
                          );
                      })}}
                  }

                  {{Summary("Attempts to convert the value contained by the union to the provided type.")}}
                  {{Param("value", static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"The converted value if the conversion is possible; otherwise, {Langword("default")}.");
                  })}}
                  {{TypeParam("TVariant", "The type to convert the unions value to.")}}{{Create(mayBeNull, static (f, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      if (!f)
                      {
                          return;
                      }

                      b.Append($"""

                                {Remarks(static (b, ct) =>
                                {
                                    ct.ThrowIfCancellationRequested();

                                    b.Append($"Because this union may have a nullable value, {ParamRef("value")} is not guaranteed to be a non-{Langword("null")} value upon returning {Langword("true")}.");
                                })}
                                """);
                  })}}
                  {{Returns(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"{Langword("true")} if the conversion is possible; otherwise, {Langword("false")}.");
                  })}}
                  public bool TryCastTo<TVariant>({{Create(mayBeNull, static (f, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      if (f)
                      {
                          return;
                      }

                      b.Append($"[{typeof(NotNullWhenAttribute)}(true)] ");
                  })}}out TVariant? value)
                  {
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          var annotation = v.Type is { IsNullable: true } or { Kind: VariantTypeKind.Unknown } ? "?" : String.Empty;

                          b.Append(
                              $$"""
                                if(typeof(TVariant) == typeof({{v.Type.NullableValueTypeName}}) || typeof(TVariant).IsAssignableFrom(typeof({{v.Type.NullableValueTypeName}})))
                                {
                                    // The jit will optimize this conversion and prevent boxing.
                                    value = (TVariant{{annotation}})(object{{annotation}}){{new VariantAccessorComponent(v)}};
                                    
                                    return true;
                                }
                                else
                                {
                                    value = default;
                                    return false;
                                }
                                """
                          );
                      })}}
                  }

                  {{Summary("Gets a value indicating whether the unions variant is convertible to the specified type.")}}
                  {{TypeParam("TVariant", "The type to check against the variant of the union.")}}
                  {{Returns(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"{Langword("true")} if the variant of the union is convertible to {TypeParamRef("TVariant")}; otherwise, {Langword("false")}.");
                  })}}
                  public bool Is<TVariant>()
                  {
                      {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"""
                                    var result = typeof(TVariant) == typeof({v.Type.NullableValueTypeName}) || typeof(TVariant).IsAssignableFrom(typeof({v.Type.NullableValueTypeName}));
                                                 
                                    return result;
                                    """);
                      })}}
                  }

                  {{List(m.Variants, static (v, i, l, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append(
                          $$"""
                            {{Summary(v, static (v, b, ct) =>
                            {
                                ct.ThrowIfCancellationRequested();

                                b.Append($"Attempt to get the represented value if the union is of the {C(v.Name)} variant.");
                            })}}                            
                            {{Param("value", v, static (v, b, ct) =>
                            {
                                ct.ThrowIfCancellationRequested();

                                b.Append($"The unions value if the union is of the {C(v.Name)} variant; otherwise, {(v.Type.Kind is VariantTypeKind.Reference ? Langword("null") : Langword("default"))}.");
                            })}}
                            {{Remarks(v, static (v, b, ct) =>
                            {
                                ct.ThrowIfCancellationRequested();

                                b.Append($"""
                                          If the union is not of the {C(v.Name)} variant, no conversion is attempted.  
                                          In order to cast to a specific type, use {Cref("CastTo{TVariant}")} instead.
                                          """);
                            })}}
                            {{Returns(v, static (v, b, ct) =>
                            {
                                ct.ThrowIfCancellationRequested();

                                b.Append($"{Langword("true")} if the union is of the {C(v.Name)}; otherwise, {Langword("false")}.");
                            })}}
                            public bool TryCastTo{{v.Name}}({{(v.Type is { IsNullable: false, Kind: VariantTypeKind.Reference } ? "[global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]" : String.Empty)}}out {{v.Type.NullableName}}{{(v.Type is { IsNullable: false, Kind: VariantTypeKind.Reference } ? "?" : String.Empty)}} value)
                            {
                                if(Is{{v.Name}})
                                {
                                    value = CastTo{{v.Name}};
                                    return true;
                                }
                                
                                value = default;
                                return false;
                            }
                            """
                      );
                  }, "\n")}}
                  """
            );
        });

        var region = Region("Inspection", methods);

        builder.Append(region);
    }
}
