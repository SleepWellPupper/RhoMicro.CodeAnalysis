// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Runtime.InteropServices;
using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct UnmanagedVariantsContainerComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var body = Create(
            Model,
            static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                foreach (var variant in m.Variants)
                {
                    if (variant.Type.Kind is not VariantTypeKind.Unmanaged)
                    {
                        continue;
                    }

                    b.AppendLine(
                        $$"""
                          {{Summary("Initializes a new instance.")}}
                          {{Param("value", "The value to store.")}}
                          public UnmanagedVariantsContainer({{variant.Type.NullableName}} value)
                          {
                              {{variant.Name}} = value;
                          }
                          {{Summary(variant, static (v, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                                  b.Append($"Gets the stored value typed as {Cref(v.Type.DocsId)}, to be used when representing the {C(v.Name)} variant.");
                          })}}
                          {{Create(m, static (m, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              if (m.TypeParameters is not [])
                              {
                                  return;
                              }

                              b.Append(Attribute(TypeName<FieldOffsetAttribute>(), PositionalAttributeArgument("0")));
                          })}}
                          public readonly {{variant.Type.NullableName}} {{variant.Name}};
                          """
                    );
                }
            });

        var type = Create((Model, body), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, body) = t;

            b.Append($"""
                      {Summary(model, static (m, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append($"Stores unmanaged variants of {Cref(m.DocsCommentId)}.");
                      })}
                      {Type(
                          "private readonly struct",
                          "UnmanagedVariantsContainer",
                          body,
                          attributes:
                          model.TypeParameters is []
                              ?
                              [
                                  Attribute(
                                      TypeName<StructLayoutAttribute>(),
                                      AttributeArgumentComponent.CreatePositionalMemberAccess(
                                          TypeName<LayoutKind>(),
                                          nameof(LayoutKind.Explicit)))
                              ]
                              : [])}
                      """);
        });

        var region = Region("UnmanagedVariantsContainer", type);

        builder.Append(region);
    }
}
