// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics.CodeAnalysis;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct FactoryComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = Create(
            Model,
            static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.AppendLine(
                    $$"""
                      {{Summary(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"Attempts to create an instance of {Cref(m.DocsCommentId)}.");
                      })}}
                      {{Param("value", m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The value to create an instance of {Cref(m.DocsCommentId)} from.");
                      })}}
                      {{Param("union", m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append(
                              $"""
                               The instance of {Cref(m.DocsCommentId)} if one could be created; otherwise, 
                               {(m.TypeKind is UnionTypeKind.Class ? Langword("null") : Langword("default"))}.
                               """);
                      })}}
                      {{TypeParam("TVariant", m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The type of the value to create an instance of {Cref(m.DocsCommentId)} from.");
                      })}}
                      {{Remarks(static (b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"If more than one variant of the union implement {TypeParamRef("TVariant")}, the selected variant is not specified and may change in future versions.");
                      })}}
                      {{Returns(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"{Langword("true")} if an instance of {Cref(m.DocsCommentId)} could be created; otherwise, {Langword("false")}.");
                      })}}
                      public {{typeof(Boolean)}} TryCreate<TVariant>(
                          TVariant value,
                          [{{TypeName<NotNullWhenAttribute>()}}(true)]
                          out {{new UnionTypeNameComponent(m, RenderNullable: true)}} union)
                      {
                          switch (value)
                          {
                              {{Create(m, static (m, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  var nullCaseHandled = false;

                                  for (var i = 0; i < m.Variants.Count; i++)
                                  {
                                      ct.ThrowIfCancellationRequested();
                                      var variant = m.Variants[i];

                                      if (i is not 0)
                                      {
                                          b.AppendLine();
                                      }

                                      b.Append($"case {variant.Type.Name} v: return {new UnionTypeNameComponent(m)}.TryCreateFrom{variant.Name}(v, out union);");

                                      if (!variant.Type.IsNullable || nullCaseHandled) {
                                          continue;
                                      }

                                      b.AppendLine()
                                          .Append($"case null: return {new UnionTypeNameComponent(m)}.TryCreateFrom{variant.Name}(({variant.Type.NullableName})null, out union);");
                                          
                                      nullCaseHandled = true;
                                  }
                              })}}
                              case {{new UnionTypeNameComponent(m)}} v:
                                  union = new {{new UnionTypeNameComponent(m)}}(v);
                                  return true;
                              case {{m.TypeNames.IUnion}} v: return v.TryMapTo(this, out union);
                              default:
                                  union = default;
                                  return false;
                          }
                      }
                      
                      {{Summary(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"Creates an instance of {Cref(m.DocsCommentId)}.");
                      })}}
                      {{Param("value", m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The value to create an instance of {Cref(m.DocsCommentId)} from.");
                      })}}
                      {{TypeParam("TVariant", m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The type of the value to create an instance of {Cref(m.DocsCommentId)} from.");
                      })}}
                      {{Remarks(static (b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"If more than one variant of the union implement {TypeParamRef("TVariant")}, the selected variant is not specified and may change in future versions.");
                      })}}
                      {{Returns(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The new instance of {Cref(m.DocsCommentId)}.");
                      })}}
                      public {{new UnionTypeNameComponent(m)}} Create<TVariant>(TVariant value)
                      {
                          switch(value)
                          {
                              {{Create(m, static (m, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  var nullCaseHandled = false;
                                  
                                  for (var i = 0; i < m.Variants.Count; i++)
                                  {
                                      ct.ThrowIfCancellationRequested();
                                      var variant = m.Variants[i];

                                      if (i is not 0)
                                      {
                                          b.AppendLine();
                                      }

                                      b.Append($"case {variant.Type.Name} v: return new {new UnionTypeNameComponent(m)}(v);");

                                      if (!variant.Type.IsNullable || nullCaseHandled) {
                                          continue;
                                      }

                                      b.AppendLine()
                                          .Append($"case null: return new {new UnionTypeNameComponent(m)}(({variant.Type.NullableName})null);");

                                      nullCaseHandled = true;
                                  }
                              })}}
                              case {{new UnionTypeNameComponent(m)}} v: return new {{new UnionTypeNameComponent(m)}}(v);
                              case {{m.TypeNames.IUnion}} v: return v.MapTo<{{new UnionTypeNameComponent(m)}}, Factory>(this);
                              default:
                              throw new {{typeof(ArgumentOutOfRangeException)}}(nameof(value), value, $"Unable to create an instance of '{typeof({{new UnionTypeNameComponent(m)}})}' from a value of type '{value?.GetType() ?? typeof(TVariant)}': {(value is null ? "null" : $"'{value}'")}");
                          }
                      }
                      """
                );
            });

        var type = Create((Model, members), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, members) = t;

            b.Append($"""
                      {Summary(model, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"Creates instances of {Cref(m.DocsCommentId)}.");
                      })}
                      {Type(
                          "private readonly struct",
                          "Factory",
                          members,
                          baseTypeList: [model.TypeNames.IUnionFactory])}
                      """);
        });

        var region = Region("Factory", type);

        builder.Append(region);
    }
}
