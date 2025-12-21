// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

partial class JanusAnalyzer
{
    readonly partial record struct AttributeAnalysisContext(
        SemanticModel SemanticModel,
        IObjectCreationOperation AttributeOperation,
        AttributeSyntax AttributeSyntax,
        INamedTypeSymbol AttributeSymbol,
        ISymbol TargetSymbol)
    {
        public Boolean TryGetUnionTypeSymbol([NotNullWhen(true)] out INamedTypeSymbol? unionTypeSymbol)
        {
            switch (TargetSymbol)
            {
                case INamedTypeSymbol target:
                    unionTypeSymbol = target;
                    return true;
                case ITypeParameterSymbol parameter:
                    unionTypeSymbol = parameter.ContainingType;
                    return true;
                default:
                    unionTypeSymbol = null;
                    return false;
            }
        }

        [TypeSymbolPattern(typeof(UnionTypeSettingsAttribute))]
        private static partial Boolean IsUnionTypeSettingsAttribute(ITypeSymbol? type);

        private static Boolean IsUnionTypeAttribute(ITypeSymbol? type)
        {
            var result = type is INamedTypeSymbol
            {
                Name: "UnionTypeAttribute",
                TypeArguments.Length: < 9,
                ContainingNamespace:
                {
                    Name: "CodeAnalysis",
                    ContainingNamespace:
                    {
                        Name: "RhoMicro",
                        ContainingNamespace:
                        {
                            IsGlobalNamespace: true
                        }
                    }
                }
            };

            return result;
        }

        public static Boolean TryCreateForUnionTypeSettingsAttribute(
            OperationAnalysisContext ctx,
            out AttributeAnalysisContext attributeAnalysisContext) =>
            TryCreateForUnionTypeSettingsAttribute(
                ctx.Operation,
                GetContainingSymbol(ctx),
                out attributeAnalysisContext,
                ctx.CancellationToken);

        public static Boolean TryCreateForUnionTypeSettingsAttribute(
            IOperation operation,
            ISymbol containingSymbol,
            out AttributeAnalysisContext attributeAnalysisContext,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            attributeAnalysisContext = default;

            if (operation is not IAttributeOperation
                {
                    Operation: IObjectCreationOperation
                    {
                        Type: INamedTypeSymbol attributeSymbol
                    } attributeOperation
                })
            {
                return false;
            }

            if (!IsUnionTypeSettingsAttribute(attributeSymbol))
            {
                return false;
            }

            if (attributeOperation.Syntax is not AttributeSyntax attributeSyntax)
            {
                return false;
            }

            if (attributeOperation.SemanticModel is not { } semanticModel)
            {
                return false;
            }

            attributeAnalysisContext = new AttributeAnalysisContext(
                semanticModel,
                attributeOperation,
                attributeSyntax,
                AttributeSymbol: attributeSymbol,
                TargetSymbol: containingSymbol);

            return true;
        }

        public static Boolean TryCreateForUnionTypeAttribute(
            OperationAnalysisContext ctx,
            out AttributeAnalysisContext attributeAnalysisContext) =>
            TryCreateForUnionTypeAttribute(
                ctx.Operation,
                GetContainingSymbol(ctx),
                out attributeAnalysisContext,
                ctx.CancellationToken);

        private static ISymbol GetContainingSymbol(OperationAnalysisContext ctx)
        {
            var ct = ctx.CancellationToken;

            ct.ThrowIfCancellationRequested();

            if (ctx is
                    not
                    {
                        Operation.Syntax.Parent.Parent: TypeParameterSyntax
                        {
                            Parent: TypeParameterListSyntax
                            {
                                Parameters: var typeParameterSyntaxes
                            }
                        } typeParameterSyntax,
                        ContainingSymbol: INamedTypeSymbol
                        {
                            TypeParameters: [_, ..] typeParameterSymbols
                        }
                    } || typeParameterSyntaxes.Count != typeParameterSymbols.Length)
            {
                return ctx.ContainingSymbol;
            }

            var index = 0;
            for (; index < typeParameterSyntaxes.Count; index++)
            {
                ct.ThrowIfCancellationRequested();

                if (typeParameterSyntaxes[index].Equals(typeParameterSyntax))
                {
                    return typeParameterSymbols[index];
                }
            }

            return ctx.ContainingSymbol;
        }

        public static Boolean TryCreateForUnionTypeAttribute(
            IOperation operation,
            ISymbol containingSymbol,
            out AttributeAnalysisContext attributeAnalysisContext,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            attributeAnalysisContext = default;

            if (operation is not IAttributeOperation
                {
                    Operation: IObjectCreationOperation
                    {
                        Type: INamedTypeSymbol attributeSymbol
                    } attributeOperation
                })
            {
                return false;
            }

            if (!IsUnionTypeAttribute(attributeOperation.Type))
            {
                return false;
            }

            if (attributeOperation.Syntax is not AttributeSyntax attributeSyntax)
            {
                return false;
            }

            if (attributeOperation.SemanticModel is not { } semanticModel)
            {
                return false;
            }

            attributeAnalysisContext = new AttributeAnalysisContext(
                semanticModel,
                attributeOperation,
                attributeSyntax,
                AttributeSymbol: attributeSymbol,
                TargetSymbol: containingSymbol);

            return true;
        }
    }
}
