// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics.CodeAnalysis;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct FactoriesComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.AppendLine(
                $$"""
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
                  public static {{new UnionTypeNameComponent(m)}} Create<TVariant>(
                      TVariant value) 
                      => new Factory().Create<TVariant>(value);
                      
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
                  public static bool TryCreate<TVariant>(
                      TVariant value,
                      [{{typeof(NotNullWhenAttribute)}}(true)]
                      out {{new UnionTypeNameComponent(m, RenderNullable: true)}} union)
                      => new Factory().TryCreate<TVariant>(value, out union);

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
                  {{Returns(m, static (m, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"The new instance of {Cref(m.DocsCommentId)}.");
                  })}}
                  public static {{new UnionTypeNameComponent(m)}} Create(
                      {{new UnionTypeNameComponent(m)}} value) 
                      => new(value);
                      
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
                  {{Returns(m, static (m, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"{Langword("true")} if an instance of {Cref(m.DocsCommentId)} could be created; otherwise, {Langword("false")}.");
                  })}}
                  public static bool TryCreate(
                      {{new UnionTypeNameComponent(m)}} value,
                      [{{typeof(NotNullWhenAttribute)}}(true)]
                      out {{new UnionTypeNameComponent(m, RenderNullable: true)}} union)
                  {
                      union = Create(value);
                      
                      return true;
                  }

                  """
            );

            for (var i = 0; i < m.Variants.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                if (i is not 0)
                {
                    b.AppendLine().AppendLine();
                }

                var variant = m.Variants[i];

                b.Append(
                    $$"""
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
                      {{Returns(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"The new instance of {Cref(m.DocsCommentId)}.");
                      })}}
                      public static {{new UnionTypeNameComponent(m)}} Create(
                          {{variant.Type.NullableName}} value)
                          => new {{new UnionTypeNameComponent(m)}}(value, validate: true);
                          
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
                      {{Returns(m, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"{Langword("true")} if an instance of {Cref(m.DocsCommentId)} could be created; otherwise, {Langword("false")}.");
                      })}}
                      public static bool TryCreate(
                          {{variant.Type.NullableName}} value,
                          [{{typeof(NotNullWhenAttribute)}}(true)]
                          out {{new UnionTypeNameComponent(m, RenderNullable: true)}} union)
                      {
                          var isValid = true;
                          Validate(value, throwIfInvalid: false, ref isValid);
                          union = isValid 
                              ? new {{new UnionTypeNameComponent(m)}}(value, validate: false) 
                              : default;
                              
                          return isValid;
                      }
                      """
                );
            }
        });

        var region = Region("Factories", methods);

        builder.Append(region);
    }
}
