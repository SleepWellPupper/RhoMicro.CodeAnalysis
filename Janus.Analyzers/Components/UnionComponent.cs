// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct UnionComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        var body = Create(
            Model,
            static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if (m.Settings.JsonConverterSetting is JsonConverterSetting.EmitJsonConverter)
                {
                    b.AppendLine(new JsonConverterComponent(m));
                }

                b.AppendLine(
                    $"""
                     {new VariantGroupKindsComponent(m)}
                     {new VariantGroupModelComponent(m)}
                     {new VariantKindComponent(m)}
                     {new VariantModelComponent(m)}
                     {new FactoryComponent(m)}
                     """);

                if (ContainsUnmanagedVariants(m))
                {
                    b.AppendLine(new UnmanagedVariantsContainerComponent(m));
                }

                b.Append(
                    $"""
                     {new ConstructorComponent(m)}
                     {new FieldsComponent(m)}
                     {new PropertiesComponent(m)}
                     {new SwitchComponent(m)}
                     {new InspectionsComponent(m)}
                     {new ValidationComponent(m)}
                     {new FactoriesComponent(m)}
                     {new MappingComponent(m)}
                     {new EqualityComponent(m)}
                     {new OperatorsComponent(m)}
                     """);
            });

        var type = Create((Model, body), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, body) = t;

            if (model.EmitDocsComment)
            {
                b.AppendLine(Summary(model, static (m, b, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    b.Append($"""
                              Implements a tagged union for the following variant types:
                              {List("table", m, static (m, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append(ListHeader("Name", "Type and Description"));

                                  for (var i = 0; i < m.Variants.Count; i++)
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      var variant = m.Variants[i];
                                      b.Append($"""

                                                {Item(variant, static (v, b, ct) =>
                                                {
                                                    ct.ThrowIfCancellationRequested();

                                                    b.Append(v.Name);
                                                }, static (v, b, ct) =>
                                                {
                                                    ct.ThrowIfCancellationRequested();

                                                    if (v.Description is [_, ..])
                                                    {
                                                        b.Append("Type: ");
                                                    }

                                                    b.Append(Cref(v.Type.DocsId));

                                                    if (v.Description is [_, ..])
                                                    {
                                                        b.Append($"""
                                                                  {Br()}
                                                                  Description: {v.Description}
                                                                  """);
                                                    }
                                                })}
                                                """);
                                  }
                              })}
                              """);
                }));
            }

            if (model.Settings.JsonConverterSetting is JsonConverterSetting.EmitJsonConverter)
            {
                b.AppendLine(
                    $"[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof({new UnionTypeNameComponent(model, RenderOpenGeneric: true)}.JsonConverter))]");
            }

            b.Append(Type(
                model.TypeKind is UnionTypeKind.Class ? "partial class" : "partial struct",
                model.Name,
                body,
                baseTypeList:
                [
                    model.TypeNames.IUnion,
                    TypeName(
                        $"global::System.IEquatable<{new UnionTypeNameComponent(model)}>")
                ],
                typeParameters: [..model.TypeParameters]));
        });

        var containingTypes = Create((Model, type), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, type) = t;

            foreach (var containingType in model.ContainingTypes)
            {
                b.Append($"partial {containingType.Modifier} {containingType.Name}");
                if (containingType.TypeParameters is not [])
                {
                    b.Append('<')
                        .Append(List(containingType.TypeParameters, separator: ", "))
                        .Append('>');
                }

                b.AppendLine().AppendLine('{').Indent();
            }

            b.AppendLine(type);

            for (var i = 0; i < model.ContainingTypes.Count; i++)
            {
                b.Detent().AppendLine('}');
            }
        });

        var @namespace = Namespace(
            Model.Namespace,
            containingTypes);

        builder.Append(@namespace);
    }

    private static Boolean ContainsUnmanagedVariants(UnionModel m)
    {
        foreach (var variant in m.Variants)
        {
            if (variant.Type.Kind is VariantTypeKind.Unmanaged)
            {
                return true;
            }
        }

        return false;
    }
}
