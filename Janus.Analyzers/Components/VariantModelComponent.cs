// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct VariantModelComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append(
                $$"""
                  {{Summary("Initializes a new instance.")}}
                  {{Param("kind", "The kind of variant represented.")}}
                  public VariantModel(VariantKind kind)
                  {
                     Kind = kind;
                  }

                  {{Summary("Gets the kind of variant represented.")}}
                  public VariantKind Kind { get; }

                  {{Summary(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();
                      b.Append($"Gets a variant model representing {Cref("VariantKind.Unknown")}.");
                  })}}
                  public static VariantModel Unknown { get; } = new(VariantKind.Unknown);

                  {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($$"""
                                 {{Summary(v.Name, static (n, b, ct) =>
                                 {
                                     ct.ThrowIfCancellationRequested();
                                     b.Append($"Gets a variant model representing {Cref(n, static (n, b, ct) =>
                                     {
                                         ct.ThrowIfCancellationRequested();

                                         b.Append($"VariantKind.{n}");
                                     })}.");
                                 })}}
                                 public static VariantModel {{v.Name}} { get; } = new(VariantKind.{{v.Name}});
                                 """);
                  }, "\n")}}

                  {{Summary("Gets the set of all variants.")}}
                  public static global::System.Collections.Immutable.ImmutableArray<VariantModel> AllValues =>
                  [
                      {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append(v.Name);
                      }, ",\n")}}
                  ];

                  {{Summary("Gets the name of the variant.")}}
                  public string Name
                  {
                      get
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append($"return nameof(VariantKind.{v.Name});");
                              },
                              VariantExpression: "Kind",
                              Default: static (_, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append("throw CreateInvalidKindException();");
                              })}}
                      }
                  }

                  {{Summary("Gets the type of the variant.")}}
                  public global::System.Type Type
                  {
                      get
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append($"return typeof({(v.Type.Kind is VariantTypeKind.Reference ? v.Type.Name : v.Type.NullableName)});");
                              },
                              VariantExpression: "Kind",
                              Default: static (_, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append("throw CreateInvalidKindException();");
                              })}}
                      }
                  }

                  {{Summary("Gets the groups the variant is a part of.")}}
                  public VariantGroupModel Group
                  {
                      get
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append($"return new VariantGroupModel({List(v.Groups, static (g, i, _, b, ct) =>
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      if (i is not 0)
                                      {
                                          b.Append(" | ");
                                      }

                                      b.Append($"VariantGroupKinds.{g}");
                                  })});");
                              },
                              VariantExpression: "Kind",
                              Default: static (_, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append("throw CreateInvalidKindException();");
                              })}}
                      }
                  }

                  private {{typeof(InvalidOperationException)}} CreateInvalidKindException()
                      => new($"The {nameof(VariantModel)} instance was not initialized correctly and is holding an invalid value: {Kind}");
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

                          b.Append($"Models and provides strongly typed access to the variants of {Cref(m.DocsCommentId)}.");
                      })}
                      {Type(
                          "public readonly record struct",
                          "VariantModel",
                          members)}
                      """);
        });

        var region = Region("VariantModel", type);

        builder.Append(region);
    }
}
