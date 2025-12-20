// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

/// <summary>
/// Generates implementations for partial methods for checking if an <see cref="ITypeSymbol"/> corresponds to a given type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public partial class TypeSymbolPatternGenerator : IIncrementalGenerator
{
    // [TypeSymbolPattern(typeof(ITypeSymbol))]
    // private static partial Boolean IsTypeSymbol(ITypeSymbol type);
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "RhoMicro.CodeAnalysis.TypeSymbolPatternAttribute",
                static (n, _) => n is MethodDeclarationSyntax
                {
                    Modifiers: [.., { RawKind: (Int32)SyntaxKind.PartialKeyword }]
                },
                static (ctx, ct) =>
                {
                    ct.ThrowIfCancellationRequested();

                    // The target must be partial, return bool, and have single parameter of type ITypeSymbol.
                    if (ctx.TargetSymbol is not IMethodSymbol
                        {
                            ReturnType.SpecialType: SpecialType.System_Boolean,
                            IsPartialDefinition: true,
                            Parameters: [{ Type: {
                                Name: nameof(ITypeSymbol),
                                ContainingNamespace:
                                {
                                    Name: "CodeAnalysis",
                                    ContainingNamespace:
                                    {
                                        Name: "Microsoft",
                                        ContainingNamespace.IsGlobalNamespace: true
                                    }
                                }
                            } }
                            ]
                        } target)
                    {
                        return null;
                    }

                    using var modelCtx = ModelCreationContext.CreateDefault(ct);
                    var result = TypeSymbolPatternMethodsPartialModel.Create(target, ctx.Attributes[0], in modelCtx);

                    return result;
                }).Where(static m => m is not null)
            .Collect()
            .SelectMany(static (m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                using var ctx = ModelCreationContext.CreateDefault(ct);
                var result = TypeSymbolPatternMethodsModel.CreateAll(m!, in ctx);

                return result;
            })
            .Select(static (m, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var sourceBuilder = new IndentedStringBuilder(IndentedStringBuilderOptions.GeneratedFile with
                {
                    GeneratorName = typeof(TypeSymbolPatternGenerator).FullName, AmbientCancellationToken = ct
                });

                m.ContainingType.BuildStrings(sourceBuilder, out var hintName, out _, ct);

                foreach (var method in m.Methods)
                {
                    AppendPatternMethods(sourceBuilder, method, ct);
                }

                var source = sourceBuilder
                    .CloseAllBlocks()
                    .ToString();

                return (hintName, source);
            });

        context.RegisterSourceOutput(provider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
    }

    private static void AppendPatternMethods(IndentedStringBuilder sourceBuilder, TypeSymbolPatternMethodModel method,
                                             CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        AppendPatternMethod(sourceBuilder, method, ct);
        AppendPatternOutMethod(sourceBuilder, method, ct);
    }

    private static void AppendPatternOutMethod(IndentedStringBuilder sourceBuilder, TypeSymbolPatternMethodModel method,
                                               CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        sourceBuilder.AppendCore(SyntaxFacts.GetText(method.Accessibility));

        if (method.IsStatic)
        {
            sourceBuilder.AppendCore(" static");
        }

        sourceBuilder
            .Append(" bool ").Append(method.MethodName).AppendCore(
                "(global::Microsoft.CodeAnalysis.ITypeSymbol? type, [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ");

        AppendPatternTypeType(sourceBuilder, method, ct);

        sourceBuilder
            .Append("? outType)")
            .OpenBracesBlock()
            .Append("var result = ").Append(method.MethodName).AppendLine("(type);")
            .AppendCore("outType = result ? (");

        AppendPatternTypeType(sourceBuilder, method, ct);

        sourceBuilder
            .AppendLine(") type : null;")
            .Append("return result;")
            .CloseBlockCore();
    }

    private static void AppendPatternMethod(IndentedStringBuilder sourceBuilder, TypeSymbolPatternMethodModel method,
                                            CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        sourceBuilder.AppendCore(SyntaxFacts.GetText(method.Accessibility));

        if (method.IsStatic)
        {
            sourceBuilder.AppendCore(" static");
        }

        sourceBuilder
            .Append(" partial bool ").Append(method.MethodName)
            .AppendCore("(global::Microsoft.CodeAnalysis.ITypeSymbol? type");

        sourceBuilder
            .Append(')')
            .OpenBracesBlock()
            .AppendCore("var result = type is ");

        var (checkTypeArguments, type) = method.Type;

        AppendTypePattern(sourceBuilder, type, checkTypeArguments, ref ct);

        sourceBuilder
            .Append(';').AppendLineCore();

        sourceBuilder
            .Append("return result;")
            .CloseBlockCore();
    }

    private static void AppendTypePattern(IndentedStringBuilder sourceBuilder, TypeModel type,
                                          Boolean checkTypeArguments, ref CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (type is NamedTypeModel named)
        {
            sourceBuilder
                .Append("global::Microsoft.CodeAnalysis.INamedTypeSymbol")
                .OpenBracesBlock()
                .Append("Name: \"").Append(type.Name).Append("\",").AppendLineCore();

            if (checkTypeArguments)
            {
                _ = sourceBuilder
                    .Append("TypeArguments:")
                    .OpenBracketsBlock();

                for (var i = 0; i < named.TypeArguments.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    if (i != 0)
                    {
                        sourceBuilder.AppendCore(", ");
                    }

                    var arg = named.TypeArguments[i];
                    AppendTypePattern(sourceBuilder, arg, checkTypeArguments, ref ct);
                }

                sourceBuilder
                    .CloseBlock()
                    .Append(',')
                    .AppendLineCore();
            }

            AppendNamespacePatternPart(sourceBuilder, type, ct);
        }
        else if (type is ArrayTypeModel array)
        {
            sourceBuilder
                .Append("global::Microsoft.CodeAnalysis.IArrayTypeSymbol")
                .OpenBracesBlock()
                .AppendCore("ElementType: ");

            AppendTypePattern(sourceBuilder, array.ElementType, checkTypeArguments, ref ct);

            sourceBuilder.AppendLineCore();
        }
        else
        {
            sourceBuilder
                .OpenBracesBlock()
                .Append("Name: \"").Append(type.Name).Append("\",").AppendLineCore();
            AppendNamespacePatternPart(sourceBuilder, type, ct);
        }

        sourceBuilder.CloseBlockCore();
    }

    private static void AppendNamespacePatternPart(IndentedStringBuilder sourceBuilder, TypeModel type,
                                                   CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        sourceBuilder
            .AppendCore("ContainingNamespace: ");

        for (var i = type.NamespaceParts.Count - 1; i > -1; i--)
        {
            ct.ThrowIfCancellationRequested();

            var part = type.NamespaceParts[i];

            sourceBuilder
                .OpenBracesBlock()
                .Append("Name: \"").Append(part).AppendLine("\",")
                .AppendCore("ContainingNamespace:");
        }

        sourceBuilder
            .OpenBracesBlock()
            .AppendCore("IsGlobalNamespace: true");

        for (var i = -1; i < type.NamespaceParts.Count; i++)
        {
            sourceBuilder.CloseBlockCore();
        }
    }

    private static void AppendPatternTypeType(IndentedStringBuilder sourceBuilder, TypeSymbolPatternMethodModel method,
                                              CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (method.Type.Type is ArrayTypeModel)
        {
            sourceBuilder.AppendCore("global::Microsoft.CodeAnalysis.IArrayTypeSymbol");
        }
        else if (method.Type.Type is NamedTypeModel)
        {
            sourceBuilder.AppendCore("global::Microsoft.CodeAnalysis.INamedTypeSymbol");
        }
    }
}
