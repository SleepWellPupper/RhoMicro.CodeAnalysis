// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis.Generated;
using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.Templating;

[Generator(LanguageNames.CSharp)]
public sealed class VisitorGenerator : IIncrementalGenerator
{
    private const Int32 _supportedAttributeArity = 8;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var modelProvider = CreateModelsProvider(context);
        var sourceProvider = modelProvider.Select((m, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var source = TemplateRenderer.Render(
                new GeneratedFileTemplate(typeof(VisitorGenerator).FullName),
                new BaseNodeTemplate(m),
                ct);
            var hintName = $"{(m.Signature.Namespace is [_, ..] n ? $"{n}." : String.Empty)}{m.Signature.Name}{(m.Signature.TypeParameters.Count is > 0 and var c ? $"`{c:0}" : String.Empty)}";

            return (hintName, source);
        });

        context.RegisterSourceOutput(sourceProvider, (ctx, t) => ctx.AddSource(t.hintName, t.source));
        IncludedFileSources.RegisterToContext(context);
    }

    private static IncrementalValuesProvider<BaseNodeModel> CreateModelsProvider(IncrementalGeneratorInitializationContext context)
    {
        var initialModelProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                typeof(GenerateVisitorAttribute).FullName,
                IsGenerateVisitorAttributeTarget,
                CreateBaseNodeModel)
            .Collect();

        var modelProvider = default(IncrementalValueProvider<EquatableList<BaseNodeModel>>?);

        for (var i = 1; i <= _supportedAttributeArity; i++)
        {
            var fullyQualifiedMetadataName = $"{typeof(GenerateVisitorAttribute).FullName}`{i:0}";

            var next = context.SyntaxProvider.ForAttributeWithMetadataName(
                    fullyQualifiedMetadataName,
                    IsGenerateVisitorAttributeTarget,
                    CreateBaseNodeModel);

            modelProvider = modelProvider is { } previous
                ? next
                    .Collect()
                    .Combine(previous)
                    .Select(AggregateModels)
                : next
                    .Collect()
                    .Combine(initialModelProvider)
                    .Select(AggregateModels);
        }

        Debug.Assert(modelProvider is not null);

        var result = modelProvider!.Value.SelectMany(static (m, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            return m;
        });

        return result;
    }

    private static EquatableList<BaseNodeModel> AggregateModels<TLeft, TRight>(
        (TLeft Left, TRight Right) t,
        CancellationToken ct)
        where TLeft : IEnumerable<BaseNodeModel?>
        where TRight : IEnumerable<BaseNodeModel?>
    {
        ct.ThrowIfCancellationRequested();

        var (left, right) = t;
        using var ctx = ModelCreationContext.CreateDefault(ct);
        var result = ctx.CollectionFactory.CreateList<BaseNodeModel>();
        var duplicatesMap = new Dictionary<NodeSignatureModel, (BaseNodeModel duplicate, Int32 resultIndex, Boolean isImmutable)>();

        var handledNodes = new HashSet<NodeSignatureModel>();

        AddModels(left, result, duplicatesMap, handledNodes, in ctx);
        AddModels(right, result, duplicatesMap, handledNodes, in ctx);

        return result;
    }

    private static void AddModels<TList>(
        TList models,
        EquatableList<BaseNodeModel> result,
        Dictionary<NodeSignatureModel, (BaseNodeModel duplicate, Int32 resultIndex, Boolean isImmutable)> duplicatesMap,
        HashSet<NodeSignatureModel> handledNodes,
        in ModelCreationContext ctx)
        where TList : IEnumerable<BaseNodeModel?>
    {
        ctx.ThrowIfCancellationRequested();

        foreach (var model in models)
        {
            ctx.ThrowIfCancellationRequested();

            if (model is null)
                continue;

            var signature = model.Signature;

            if (duplicatesMap.TryGetValue(signature, out var t))
            {
                var (duplicate, resultIndex, duplicateIsImmutable) = t;

                if (duplicateIsImmutable)
                {
                    var newDuplicate = new BaseNodeModel(ctx.CollectionFactory.CreateList<NodeModel>(), signature);

                    foreach (var node in duplicate.Nodes)
                    {
                        ctx.ThrowIfCancellationRequested();

                        if (handledNodes.Add(node.Signature))
                            newDuplicate.Nodes.Add(node);
                    }

                    duplicate = newDuplicate;

                    duplicatesMap[signature] = (duplicate, resultIndex, isImmutable: false);
                    result[resultIndex] = duplicate;
                }

                foreach (var node in model.Nodes)
                {
                    ctx.ThrowIfCancellationRequested();

                    if (handledNodes.Add(node.Signature))
                        duplicate.Nodes.Add(node);
                }
            }
            else
            {
                duplicatesMap[signature] = (duplicate: model, resultIndex: result.Count, isImmutable: true);
                result.Add(model);
            }
        }
    }

    private static BaseNodeModel? CreateBaseNodeModel(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (ctx.TargetSymbol is not INamedTypeSymbol baseNodeType)
            return null;

        if (baseNodeType.ContainingType is not null)
            return null;

        using var modelCtx = ModelCreationContext.CreateDefault(ct);
        var nodes = modelCtx.CollectionFactory.CreateList<NodeModel>();

        if (!NodeSignatureModel.TryCreate(baseNodeType, out var baseSignature, in modelCtx))
            return null;

        var handledTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var result = new BaseNodeModel(nodes, baseSignature);

        foreach (var attribute in ctx.Attributes)
        {
            ct.ThrowIfCancellationRequested();

            foreach (var arg in attribute.ConstructorArguments)
            {
                ct.ThrowIfCancellationRequested();

                if (arg is not { Kind: TypedConstantKind.Array, Values: { } @params })
                    continue;

                foreach (var param in @params)
                {
                    if (param is not { Kind: TypedConstantKind.Type, Value: ITypeSymbol typeArg })
                        continue;

                    NodeModel.AddModels(
                        typeArg,
                        baseNodeType,
                        baseSignature,
                        nodes,
                        handledTypes,
                        in modelCtx);
                }
            }

            if (attribute.AttributeClass?.TypeArguments is not [_, ..] args)
                continue;

            foreach (var arg in args)
            {
                ct.ThrowIfCancellationRequested();

                NodeModel.AddModels(
                    arg,
                    baseNodeType,
                    baseSignature,
                    nodes,
                    handledTypes,
                    in modelCtx);
            }
        }

        return result;
    }

    private static Boolean IsGenerateVisitorAttributeTarget(SyntaxNode n, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (n is not TypeDeclarationSyntax
            {
                RawKind: (Int32)SyntaxKind.RecordDeclaration or (Int32)SyntaxKind.ClassDeclaration,
                Arity: 0
            } classDeclaration)
        {
            return false;
        }

        var isAbstract = false;
        foreach (var modifier in classDeclaration.Modifiers)
        {
            ct.ThrowIfCancellationRequested();

            if (modifier.IsKind(SyntaxKind.AbstractKeyword))
            {
                if (isAbstract)
                    return false;

                isAbstract = true;
            }
        }

        if (!isAbstract)
            return false;

        return true;
    }
}
