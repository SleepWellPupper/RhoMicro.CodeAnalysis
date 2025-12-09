// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

/// <summary>
/// Generates diagnostics for guiding usage of the <see cref="JanusGenerator"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class JanusAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        DiagnosticDescriptors.ToStringSettingIgnored,
        DiagnosticDescriptors.UnionMayNotBeRecordType,
        DiagnosticDescriptors.GenericUnionsCannotBeJsonSerializable,
        DiagnosticDescriptors.NoMoreThan31VariantGroupsMayBeDefined,
        DiagnosticDescriptors.UnionMayNotBeStatic,
        DiagnosticDescriptors.VariantNamesMustBeUnique,
        DiagnosticDescriptors.EnsureValidStructUnionState,
        DiagnosticDescriptors.InterfaceVariantIsExcludedFromConversionOperators,
        DiagnosticDescriptors.VariantTypesMustBeUnique,
        DiagnosticDescriptors.ObjectCannotBeUsedAsAVariant,
        DiagnosticDescriptors.ValueTypeCannotBeUsedAsAVariantOfStructUnion,
        DiagnosticDescriptors.UnionCannotExplicitlyDefineBaseType,
        DiagnosticDescriptors.UnionCannotBeUsedAsVariantOfItself,
        DiagnosticDescriptors.PreferNullableStructOverIsNullable,
        DiagnosticDescriptors.NullableVariantNotAllowedAlongWithNonNullableVariant,
        DiagnosticDescriptors.UnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute,
        DiagnosticDescriptors.DuplicateVariantGroupNamesAreIgnored,
        DiagnosticDescriptors.ClassUnionsShouldBeSealed,
        DiagnosticDescriptors.UnionCannotBeRefStruct,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

#if !DEBUG
        context.EnableConcurrentExecution();
#endif

        context.RegisterOperationAction(
            ReportToStringSettingIgnored, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportUnionMayNotBeRecordType, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportGenericUnionsCannotBeJsonSerializable, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportNoMoreThan31VariantGroupsMayBeDefined, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportUnionMayNotBeStatic, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportVariantNamesMustBeUnique, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportEnsureValidStructUnionState, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportInterfaceVariantIsExcludedFromConversionOperators, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportVariantTypesMustBeUnique, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportObjectCannotBeUsedAsAVariant, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportValueTypeCannotBeUsedAsAVariantOfStructUnion, OperationKind.Attribute);
        context.RegisterSymbolAction(
            ReportUnionCannotExplicitlyDefineBaseType, SymbolKind.NamedType);
        context.RegisterOperationAction(
            ReportUnionCannotBeUsedAsVariantOfItself, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportPreferNullableStructOverIsNullable, OperationKind.Attribute);
        context.RegisterOperationAction(
            ReportNullableVariantNotAllowedAlongWithNonNullableVariant, OperationKind.Attribute);
        context.RegisterSymbolAction(
            ReportUnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute, SymbolKind.NamedType);
        context.RegisterOperationAction(
            ReportDuplicateVariantGroupNamesAreIgnored, OperationKind.Attribute);
        context.RegisterSymbolAction(
            ReportClassUnionsShouldBeSealed, SymbolKind.NamedType);
        context.RegisterSymbolAction(
            ReportUnionCannotBeRefStruct, SymbolKind.NamedType);
    }

    private static void ReportUnionCannotBeRefStruct(SymbolAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (ctx.Symbol is not INamedTypeSymbol
            {
                IsRefLikeType: true
            } target)
        {
            return;
        }

        var isUnion = false;
        var attributes = target.GetAttributes();

        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (!attribute.IsUnionTypeAttribute())
            {
                continue;
            }

            isUnion = true;
            break;
        }

        if (!isUnion)
        {
            return;
        }

        var unionName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        foreach (var reference in target.DeclaringSyntaxReferences)
        {
            ct.ThrowIfCancellationRequested();

            if (reference.GetSyntax(ct) is not TypeDeclarationSyntax
                {
                    Identifier: { } identifier
                })
            {
                continue;
            }

            var location = identifier.GetLocation();
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnionCannotBeRefStruct,
                location,
                messageArgs:
                [
                    unionName
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportClassUnionsShouldBeSealed(SymbolAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (ctx.Symbol is not INamedTypeSymbol
            {
                IsSealed: false,
                IsReferenceType: true
            } target)
        {
            return;
        }

        var isUnion = false;

        var attributes = target.GetAttributes();
        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (attribute.IsUnionTypeAttribute())
            {
                isUnion = true;
                break;
            }
        }

        if (!isUnion)
        {
            return;
        }

        var unionName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        foreach (var reference in target.DeclaringSyntaxReferences)
        {
            ct.ThrowIfCancellationRequested();

            if (reference.GetSyntax(ct) is not TypeDeclarationSyntax
                {
                    Identifier: { } identifier
                })
            {
                continue;
            }

            var location = identifier.GetLocation();
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.ClassUnionsShouldBeSealed,
                location,
                messageArgs:
                [
                    unionName
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportDuplicateVariantGroupNamesAreIgnored(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (attributeContext is not
            {
                AttributeOperation.Initializer.Initializers: [_, ..] initializers,
                AttributeSymbol:
                {
                    TypeArguments: [_, ..]
                }
            })
        {
            return;
        }

        foreach (var initializer in initializers)
        {
            ct.ThrowIfCancellationRequested();

            if (initializer is not ISimpleAssignmentOperation
                {
                    Target: IPropertyReferenceOperation
                    {
                        Member.Name: nameof(UnionTypeAttribute.Groups)
                    },
                    Value: { } value
                })
            {
                continue;
            }

            var elements = value switch
            {
                IConversionOperation
                {
                    Operand: ICollectionExpressionOperation
                    {
                        Elements: { } e
                    }
                } => e,
                IArrayCreationOperation
                {
                    Initializer.ElementValues: { } e
                } => e,
                _ => []
            };

            if (elements is [] or [_])
            {
                continue;
            }

            var groups = new HashSet<String>();

            foreach (var element in elements)
            {
                ct.ThrowIfCancellationRequested();

                if (element.ConstantValue is not { HasValue: true, Value: String group })
                {
                    continue;
                }

                if (groups.Add(group))
                {
                    continue;
                }

                var location = element.Syntax.GetLocation();
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.DuplicateVariantGroupNamesAreIgnored,
                    location,
                    messageArgs:
                    [
                        group
                    ]);
                ctx.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void ReportUnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute(SymbolAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (ctx.Symbol is not INamedTypeSymbol target)
        {
            return;
        }

        var attributes = target.GetAttributes();
        var settingsLocation = Location.None;

        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (attribute.IsUnionTypeAttribute())
            {
                return;
            }

            if (attribute.IsUnionTypeSettingsAttribute())
            {
                settingsLocation = attribute.ApplicationSyntaxReference?
                    .GetSyntax(ct)
                    .GetLocation() ?? Location.None;
            }
        }

        foreach (var typeParameter in target.TypeParameters)
        {
            ct.ThrowIfCancellationRequested();

            var typeParameterAttributes = typeParameter.GetAttributes();

            foreach (var attribute in typeParameterAttributes)
            {
                ct.ThrowIfCancellationRequested();

                if (attribute.IsUnionTypeAttribute())
                {
                    return;
                }
            }
        }

        if (settingsLocation == Location.None)
        {
            return;
        }

        var unionName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.UnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute,
            settingsLocation,
            messageArgs:
            [
                unionName
            ]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private static void ReportNullableVariantNotAllowedAlongWithNonNullableVariant(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target)
         || attributeContext is not
            {
                AttributeSymbol.TypeArguments: [_, ..] localTypeArgumentSymbols,
                AttributeSyntax.Name: GenericNameSyntax
                {
                    TypeArgumentList.Arguments: [_, ..] localTypeArgumentSyntaxes
                }
            })
        {
            return;
        }

        if (localTypeArgumentSyntaxes.Count != localTypeArgumentSymbols.Length)
        {
            return;
        }

        var typeLocations = new Dictionary<ITypeSymbol, List<Location>>(SymbolEqualityComparer.Default);

        for (var i = 0; i < localTypeArgumentSymbols.Length; i++)
        {
            var typeArgumentSymbol = localTypeArgumentSymbols[i];
            var location = localTypeArgumentSyntaxes[i].GetLocation();

            if (typeArgumentSymbol is INamedTypeSymbol
                {
                    OriginalDefinition:
                    {
                        SpecialType: SpecialType.System_Nullable_T
                    },
                    TypeArguments: [{ } actualTypeArgumentSymbol]
                })
            {
                typeArgumentSymbol = actualTypeArgumentSymbol;
            }

            if (!typeLocations.TryGetValue(typeArgumentSymbol, out var locations))
            {
                typeLocations.Add(typeArgumentSymbol, locations = []);
            }

            locations.Add(location);
        }

        var attributes = target.GetAttributes();
        var nullableTypeArguments = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var nonNullableTypeArguments = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (!attribute.IsUnionTypeAttribute())
            {
                continue;
            }

            if (attribute.AttributeClass is not
                {
                    TypeArguments: [_, ..] globalTypeArgumentSymbols
                })
            {
                continue;
            }

            foreach (var typeArgumentSymbol in globalTypeArgumentSymbols)
            {
                ct.ThrowIfCancellationRequested();

                var nonNullableTypeArgumentSymbol = typeArgumentSymbol;
                var isNullable = false;

                if (typeArgumentSymbol is INamedTypeSymbol
                    {
                        OriginalDefinition:
                        {
                            SpecialType: SpecialType.System_Nullable_T
                        },
                        TypeArguments: [{ } actualTypeArgumentSymbol]
                    })
                {
                    nonNullableTypeArgumentSymbol = actualTypeArgumentSymbol;
                    isNullable = true;
                }

                if (!typeLocations.ContainsKey(nonNullableTypeArgumentSymbol))
                {
                    continue;
                }

                if (isNullable)
                {
                    nullableTypeArguments.Add(nonNullableTypeArgumentSymbol);
                }
                else
                {
                    nonNullableTypeArguments.Add(nonNullableTypeArgumentSymbol);
                }
            }
        }

        var typeName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        foreach (var nullableTypeArgument in nullableTypeArguments)
        {
            ct.ThrowIfCancellationRequested();

            if (!nonNullableTypeArguments.Contains(nullableTypeArgument) ||
                !typeLocations.TryGetValue(nullableTypeArgument, out var locations))
            {
                continue;
            }

            var variantName = nullableTypeArgument.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            foreach (var location in locations)
            {
                ct.ThrowIfCancellationRequested();

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.NullableVariantNotAllowedAlongWithNonNullableVariant,
                    location,
                    messageArgs:
                    [
                        typeName,
                        variantName
                    ]);
                ctx.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void ReportPreferNullableStructOverIsNullable(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (attributeContext.AttributeSyntax.Name is not GenericNameSyntax
            {
                TypeArgumentList.Arguments: [{ } variantSyntax]
            })
        {
            return;
        }

        if (attributeContext.SemanticModel.GetTypeInfo(variantSyntax).Type is not
            {
                IsValueType: true,
                OriginalDefinition:
                {
                    SpecialType: not SpecialType.System_Nullable_T
                }
            } variantSymbol)
        {
            return;
        }

        if (attributeContext.AttributeOperation.Initializer?.Initializers is not [_, ..] initializers)
        {
            return;
        }

        var variantTypeName = variantSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var isNullableText = String.Empty;
        var location = Location.None;
        var variantName = variantSymbol.Name;

        foreach (var initializer in initializers)
        {
            ct.ThrowIfCancellationRequested();

            if (initializer is not ISimpleAssignmentOperation
                {
                    Target: IPropertyReferenceOperation
                    {
                        Member.Name: { } assignedMember
                    },
                    Value.ConstantValue: { HasValue: true, Value: { } assignedValue }
                } assignmentOperation)
            {
                continue;
            }

            if (assignedMember is nameof(UnionTypeAttribute.Name))
            {
                if (assignedValue is String explicitName)
                {
                    variantName = explicitName;
                }

                continue;
            }

            if (assignedMember is nameof(UnionTypeAttribute.IsNullable))
            {
                if (assignedValue is false)
                {
                    return;
                }

                isNullableText = assignmentOperation.Syntax.ToString();
                location = assignmentOperation.Syntax.GetLocation();
            }
        }

        if (isNullableText is [])
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PreferNullableStructOverIsNullable,
            location,
            messageArgs:
            [
                variantTypeName,
                isNullableText,
                variantName
            ]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private static void ReportUnionCannotBeUsedAsVariantOfItself(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (attributeContext is not
            {
                AttributeSyntax.Name: GenericNameSyntax
                {
                    TypeArgumentList.Arguments: [_, ..] typeArgumentSyntaxes
                }
            })
        {
            return;
        }

        for (var i = 0; i < typeArgumentSyntaxes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var typeArgumentSyntax = typeArgumentSyntaxes[i];

            if (attributeContext.SemanticModel.GetTypeInfo(typeArgumentSyntax, ct).Type is not { } variant)
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(variant, attributeContext.TargetSymbol))
            {
                continue;
            }

            var location = typeArgumentSyntaxes[i].GetLocation();
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnionCannotBeUsedAsVariantOfItself,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportUnionCannotExplicitlyDefineBaseType(SymbolAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (ctx.Symbol is not INamedTypeSymbol
            {
                IsReferenceType: true,
                BaseType:
                {
                    SpecialType: not SpecialType.System_Object
                } baseTypeSymbol
            } target)
        {
            return;
        }

        var attributes = target.GetAttributes();
        var baseTypeName = baseTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var isUnion = false;

        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (attribute.IsUnionTypeAttribute())
            {
                isUnion = true;
            }
        }

        if (!isUnion)
        {
            return;
        }

        foreach (var reference in target.DeclaringSyntaxReferences)
        {
            ct.ThrowIfCancellationRequested();

            if (reference.GetSyntax(ct) is not TypeDeclarationSyntax
                {
                    Identifier: { } identifier
                })
            {
                continue;
            }

            var location = identifier.GetLocation();
            var unionName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnionCannotExplicitlyDefineBaseType,
                location,
                messageArgs:
                [
                    unionName,
                    baseTypeName
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportValueTypeCannotBeUsedAsAVariantOfStructUnion(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target)
         || !target.IsValueType
         || attributeContext is not
            {
                AttributeSyntax.Name: GenericNameSyntax
                {
                    TypeArgumentList.Arguments: [_, ..] typeArgumentSyntaxes
                }
            })
        {
            return;
        }

        for (var i = 0; i < typeArgumentSyntaxes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var typeArgumentSyntax = typeArgumentSyntaxes[i];

            if (attributeContext.SemanticModel.GetTypeInfo(typeArgumentSyntax, ct).Type is not
                {
                    SpecialType: SpecialType.System_ValueType
                })
            {
                continue;
            }

            var location = typeArgumentSyntaxes[i].GetLocation();
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.ValueTypeCannotBeUsedAsAVariantOfStructUnion,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportObjectCannotBeUsedAsAVariant(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (attributeContext is not
            {
                AttributeSyntax.Name: GenericNameSyntax
                {
                    TypeArgumentList.Arguments: [_, ..] typeArgumentSyntaxes
                }
            })
        {
            return;
        }

        for (var i = 0; i < typeArgumentSyntaxes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var typeArgumentSyntax = typeArgumentSyntaxes[i];

            if (attributeContext.SemanticModel.GetTypeInfo(typeArgumentSyntax, ct).Type is not
                {
                    SpecialType: SpecialType.System_Object
                })
            {
                continue;
            }

            var location = typeArgumentSyntaxes[i].GetLocation();
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.ObjectCannotBeUsedAsAVariant,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportVariantTypesMustBeUnique(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target)
         || attributeContext is not
            {
                AttributeSymbol.TypeArguments: [_, ..] localTypeArgumentSymbols,
                AttributeSyntax.Name: GenericNameSyntax
                {
                    TypeArgumentList.Arguments: [_, ..] localTypeArgumentSyntaxes
                }
            })
        {
            return;
        }

        if (localTypeArgumentSyntaxes.Count != localTypeArgumentSymbols.Length)
        {
            return;
        }

        var typeLocations = new Dictionary<ITypeSymbol, List<Location>>(SymbolEqualityComparer.Default);

        for (var i = 0; i < localTypeArgumentSymbols.Length; i++)
        {
            var typeArgumentSymbol = localTypeArgumentSymbols[i];
            var location = localTypeArgumentSyntaxes[i].GetLocation();

            if (typeArgumentSymbol is INamedTypeSymbol
                {
                    OriginalDefinition:
                    {
                        SpecialType: SpecialType.System_Nullable_T
                    },
                    TypeArguments: [{ } actualTypeArgumentSymbol]
                })
            {
                typeArgumentSymbol = actualTypeArgumentSymbol;
            }

            if (!typeLocations.TryGetValue(typeArgumentSymbol, out var locations))
            {
                typeLocations.Add(typeArgumentSymbol, locations = []);
            }

            locations.Add(location);
        }

        var attributes = target.GetAttributes();
        var duplicates = new Dictionary<ITypeSymbol, Boolean>(SymbolEqualityComparer.Default);
        foreach (var attribute in attributes)
        {
            ct.ThrowIfCancellationRequested();

            if (!attribute.IsUnionTypeAttribute())
            {
                continue;
            }

            if (attribute is not
                {
                    AttributeClass.TypeArguments: [_, ..] typeArgumentSymbols
                })
            {
                continue;
            }

            foreach (var typeArgumentSymbol in typeArgumentSymbols)
            {
                ct.ThrowIfCancellationRequested();

                if (!typeLocations.ContainsKey(typeArgumentSymbol))
                {
                    continue;
                }

                if (duplicates.TryGetValue(typeArgumentSymbol, out var isDuplicate))
                {
                    if (!isDuplicate)
                    {
                        duplicates[typeArgumentSymbol] = true;
                    }
                }
                else
                {
                    duplicates[typeArgumentSymbol] = false;
                }
            }
        }

        if (duplicates.Count is 0)
        {
            return;
        }

        var unionName = target.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        foreach (var (variantType, isDuplicate) in duplicates)
        {
            ct.ThrowIfCancellationRequested();

            if (!isDuplicate)
            {
                continue;
            }

            if (!typeLocations.TryGetValue(variantType, out var locations))
            {
                continue;
            }

            foreach (var location in locations)
            {
                ct.ThrowIfCancellationRequested();

                var variantTypeName = variantType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.VariantTypesMustBeUnique,
                    location,
                    messageArgs:
                    [
                        unionName,
                        variantTypeName
                    ]);
                ctx.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void ReportInterfaceVariantIsExcludedFromConversionOperators(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (attributeContext.AttributeSyntax.Name is not GenericNameSyntax
            {
                TypeArgumentList.Arguments: { } argumentSyntaxes
            })
        {
            return;
        }

        var interfaceVariants = new Dictionary<String, List<Location>>();
        for (var i = 0; i < argumentSyntaxes.Count; i++)
        {
            var typeArgumentSyntax = argumentSyntaxes[i];

            if (attributeContext.SemanticModel.GetTypeInfo(typeArgumentSyntax, ct).Type is not INamedTypeSymbol
                {
                    TypeKind: TypeKind.Interface
                } typeArgumentSymbol)
            {
                continue;
            }

            var name = typeArgumentSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
            var location = typeArgumentSyntax.GetLocation();

            if (!interfaceVariants.TryGetValue(name, out var locations))
            {
                interfaceVariants.Add(name, locations = []);
            }

            locations.Add(location);
        }

        if (interfaceVariants.Count is 0)
        {
            return;
        }

        foreach (var (name, locations) in interfaceVariants)
        {
            ct.ThrowIfCancellationRequested();

            foreach (var location in locations)
            {
                ct.ThrowIfCancellationRequested();

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.InterfaceVariantIsExcludedFromConversionOperators,
                    location,
                    messageArgs:
                    [
                        name,
                        attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                    ]);
                ctx.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void ReportEnsureValidStructUnionState(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target)
         || target.TypeKind is TypeKind.Class)
        {
            return;
        }

        var model = UnionModel.Create(target, ct);

        if (model.Variants.Any(v => v.Type is not { Kind: VariantTypeKind.Reference, IsNullable: false }))
        {
            return;
        }

        var locations = attributeContext
            .TargetSymbol
            .DeclaringSyntaxReferences
            .Select(r => r.GetSyntax(ct))
            .OfType<TypeDeclarationSyntax>()
            .Select(d => d.Identifier.GetLocation());

        foreach (var location in locations)
        {
            ct.ThrowIfCancellationRequested();

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.EnsureValidStructUnionState,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportVariantNamesMustBeUnique(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        var localNameLocations = new Dictionary<String, List<Location>>(StringComparer.Ordinal);
        var duplicates = new Dictionary<String, Boolean>();

        if (!tryGetNamedTypeTargetNames(out var unionType) && !tryGetTypeParameterTargetNames(out unionType))
        {
            return;
        }

        getGlobalNames(unionType);

        var unionTypeName = unionType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        foreach (var (variantName, isDuplicate) in duplicates)
        {
            ct.ThrowIfCancellationRequested();

            if (!isDuplicate)
            {
                continue;
            }

            if (!localNameLocations.TryGetValue(variantName, out var locations))
            {
                continue;
            }

            foreach (var location in locations)
            {
                ct.ThrowIfCancellationRequested();

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.VariantNamesMustBeUnique,
                    location,
                    messageArgs:
                    [
                        unionTypeName,
                        variantName
                    ]);
                ctx.ReportDiagnostic(diagnostic);
            }
        }

        Boolean tryGetTypeParameterTargetNames([NotNullWhen(true)] out INamedTypeSymbol? containingUnionType)
        {
            if (attributeContext is not
                {
                    TargetSymbol: ITypeParameterSymbol
                    {
                        ContainingType: var containingType
                    } target
                })
            {
                containingUnionType = null;
                return false;
            }

            var hasExplicitName = false;
            var initializers = attributeContext.AttributeOperation.Initializer?.Initializers ?? [];
            foreach (var initializer in initializers)
            {
                ct.ThrowIfCancellationRequested();

                if (initializer is not ISimpleAssignmentOperation
                    {
                        Target: IMemberReferenceOperation
                        {
                            Member.Name: nameof(UnionTypeAttribute.Name)
                        },
                        Value.ConstantValue: { HasValue: true, Value: String explicitName }
                    } nameAssignmentOperation)
                {
                    continue;
                }

                var location = nameAssignmentOperation.Syntax.GetLocation();

                registerLocation(explicitName, location);
            }

            if (hasExplicitName)
            {
            }
            else
            {
                var name = target.Name;
                foreach (var reference in target.DeclaringSyntaxReferences)
                {
                    ct.ThrowIfCancellationRequested();

                    if (reference.GetSyntax(ct) is not TypeParameterSyntax
                        {
                            Identifier: var targetSyntaxIdentifier
                        })
                    {
                        continue;
                    }

                    var location = targetSyntaxIdentifier.GetLocation();
                    registerLocation(name, location);
                }
            }

            containingUnionType = containingType;
            return true;
        }

        Boolean tryGetNamedTypeTargetNames([NotNullWhen(true)] out INamedTypeSymbol? targetUnionType)
        {
            if (!attributeContext.TryGetUnionTypeSymbol(out var target)
             || attributeContext is not
                {
                    AttributeSymbol.TypeArguments: [_, ..] localTypeArgumentSymbols,
                    AttributeSyntax.Name: GenericNameSyntax
                    {
                        TypeArgumentList.Arguments: [_, ..] localTypeArgumentSyntaxes
                    }
                } || localTypeArgumentSyntaxes.Count != localTypeArgumentSymbols.Length)
            {
                targetUnionType = null;
                return false;
            }

            var hasExplicitName = false;
            var initializers = attributeContext.AttributeOperation.Initializer?.Initializers ?? [];
            foreach (var initializer in initializers)
            {
                ct.ThrowIfCancellationRequested();

                if (initializer is not ISimpleAssignmentOperation
                    {
                        Target: IMemberReferenceOperation
                        {
                            Member.Name: nameof(UnionTypeAttribute.Name)
                        },
                        Value.ConstantValue: { HasValue: true, Value: String explicitName }
                    } nameAssignmentOperation)
                {
                    continue;
                }

                var location = nameAssignmentOperation.Syntax.GetLocation();

                registerLocation(explicitName, location);
            }

            if (!hasExplicitName)
            {
                for (var i = 0; i < localTypeArgumentSymbols.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    var typeArgumentSymbol = localTypeArgumentSymbols[i];
                    var variantName = typeArgumentSymbol.Name;
                    var location = localTypeArgumentSyntaxes[i].GetLocation();
                    registerLocation(variantName, location);
                }
            }

            targetUnionType = target;
            return true;
        }

        void registerName(String name)
        {
            if (!localNameLocations.ContainsKey(name))
            {
                return;
            }

            if (duplicates.TryGetValue(name, out var isDuplicate))
            {
                if (!isDuplicate)
                {
                    duplicates[name] = true;
                }
            }
            else
            {
                duplicates[name] = false;
            }
        }

        void getGlobalNames(INamedTypeSymbol target)
        {
            var attributes = target.GetAttributes();

            foreach (var attribute in attributes)
            {
                ct.ThrowIfCancellationRequested();

                var typeArguments = attribute.AttributeClass?.TypeArguments ?? [];

                foreach (var typeArgument in typeArguments)
                {
                    ct.ThrowIfCancellationRequested();

                    if (!attribute.TryGetUnionTypeAttributeModel(
                        new UnionTypeAttribute.Model.TypeArgumentState(typeArgument),
                        out var model))
                    {
                        continue;
                    }

                    if (model.Name is [_, ..] name)
                    {
                        registerName(name);
                    }
                }
            }

            foreach (var typeParameter in target.TypeParameters)
            {
                ct.ThrowIfCancellationRequested();

                var typeParameterAttributes = typeParameter.GetAttributes();

                foreach (var attribute in typeParameterAttributes)
                {
                    ct.ThrowIfCancellationRequested();

                    if (!attribute.TryGetUnionTypeAttributeModel(
                        new UnionTypeAttribute.Model.TypeParameterState(typeParameter),
                        out var model))
                    {
                        continue;
                    }

                    var name = model.Name is [_, ..] explicitName
                        ? explicitName
                        : typeParameter.Name;

                    registerName(name);
                }
            }
        }

        void registerLocation(String name, Location location)
        {
            if (!localNameLocations.TryGetValue(name, out var locations))
            {
                localNameLocations.Add(name, locations = []);
            }

            locations.Add(location);
        }
    }

    private static void ReportUnionMayNotBeStatic(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        var locations = attributeContext
            .TargetSymbol
            .DeclaringSyntaxReferences
            .Select(r => r.GetSyntax(ct))
            .OfType<TypeDeclarationSyntax>()
            .Select(d => d
                .Modifiers
                .FirstOrDefault(m => m.IsKind(SyntaxKind.StaticKeyword)))
            .Where(m => m.IsKind(SyntaxKind.StaticKeyword))
            .Select(m => m.GetLocation());

        foreach (var location in locations)
        {
            ct.ThrowIfCancellationRequested();

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnionMayNotBeStatic,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private static void ReportNoMoreThan31VariantGroupsMayBeDefined(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        var groupDatum = attributeContext
            .AttributeOperation
            .Initializer?
            .Initializers
            .OfType<ISimpleAssignmentOperation>()
            .Select(i =>
            {
                if (i is not { Target: IPropertyReferenceOperation { Member.Name: nameof(UnionTypeAttribute.Groups) } })
                {
                    return (groupsOperation: i, groupsCount: -1);
                }

                if (i.Value is
                    IConversionOperation
                    {
                        Operand:
                        ICollectionExpressionOperation
                        {
                            Elements.Length: > 31 and var collectionExpressionElementCount
                        }
                    })
                {
                    return (groupsOperation: i, groupsCount: collectionExpressionElementCount);
                }

                if (i.Value is
                    IArrayCreationOperation
                    {
                        Initializer.ElementValues.Length: > 31 and var arrayInitializerElementCount
                    })
                {
                    return (groupsOperation: i, groupsCount: arrayInitializerElementCount);
                }

                return (groupsOperation: i, groupsCount: -1);
            })
            .FirstOrDefault(t => t.groupsCount is not -1);

        var (groupsOperation, groupsCount) = groupDatum.GetValueOrDefault();

        if (groupsOperation is null)
        {
            return;
        }

        var location = groupsOperation.Syntax.GetLocation();

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.NoMoreThan31VariantGroupsMayBeDefined,
            location,
            messageArgs:
            [
                attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                groupsCount
            ]);

        ctx.ReportDiagnostic(diagnostic);
    }

    private static void ReportGenericUnionsCannotBeJsonSerializable(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeSettingsAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var taget) || !taget.IsGenericType)
        {
            return;
        }

        var isJsonSerializableOperation = attributeContext
            .AttributeOperation
            .Initializer?
            .Initializers
            .OfType<ISimpleAssignmentOperation>()
            .FirstOrDefault(i => i is
            {
                Target: IPropertyReferenceOperation
                {
                    Member.Name: nameof(UnionTypeSettingsAttribute.JsonConverterSetting)
                },
                Value.ConstantValue:
                {
                    HasValue: true,
                    Value: (Int32)JsonConverterSetting.EmitJsonConverter
                }
            });

        if (isJsonSerializableOperation is null)
        {
            return;
        }

        var location = isJsonSerializableOperation.Syntax.GetLocation();
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.GenericUnionsCannotBeJsonSerializable,
            location,
            messageArgs:
            [
                attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
            ]);
        ctx.ReportDiagnostic(diagnostic);
    }

    [TypeSymbolPattern(typeof(ToStringSetting))]
    private static partial Boolean IsToStringSetting(ITypeSymbol? type);

    private static void ReportToStringSettingIgnored(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeSettingsAttribute(ctx, out var attributeContext))
        {
            return;
        }

        var toStringSettingOperation = attributeContext
            .AttributeOperation
            .Initializer?
            .Initializers
            .FirstOrDefault(i => IsToStringSetting(i.Type) && i is not ISimpleAssignmentOperation
            {
                Value.ConstantValue: { HasValue: true, Value: ToStringSetting.None }
            });

        if (toStringSettingOperation is null)
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target))
        {
            return;
        }

        var hasNonGeneratedToString = false;

        foreach (var member in target.GetMembers(nameof(ToString)))
        {
            ct.ThrowIfCancellationRequested();

            if (member is not IMethodSymbol
                {
                    Parameters: [],
                    IsOverride: true,
                } method)
            {
                return;
            }

            foreach (var reference in method.DeclaringSyntaxReferences)
            {
                ct.ThrowIfCancellationRequested();

                var declaration = reference.GetSyntax(ct);

                if (declaration.SyntaxTree.FilePath.EndsWith(".g.cs"))
                {
                    continue;
                }

                var text = declaration.SyntaxTree.GetText(ct);

                if (text.Lines is not [{ } firstLine, ..])
                {
                    continue;
                }

                if (text.GetSubText(firstLine.Span).ToString().StartsWith("// <auto-generated>"))
                {
                    continue;
                }

                hasNonGeneratedToString = true;
                break;
            }

            if (hasNonGeneratedToString)
            {
                break;
            }
        }

        if (!hasNonGeneratedToString)
        {
            return;
        }

        var location = toStringSettingOperation.Syntax.GetLocation();
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.ToStringSettingIgnored,
            location,
            messageArgs:
            [
                toStringSettingOperation.Syntax.ToString(),
                attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
            ]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private static void ReportUnionMayNotBeRecordType(OperationAnalysisContext ctx)
    {
        var ct = ctx.CancellationToken;

        ct.ThrowIfCancellationRequested();

        if (!AttributeAnalysisContext.TryCreateForUnionTypeAttribute(ctx, out var attributeContext))
        {
            return;
        }

        if (!attributeContext.TryGetUnionTypeSymbol(out var target) || !target.IsRecord)
        {
            return;
        }

        var locations = target
            .DeclaringSyntaxReferences
            .Select(r => r.GetSyntax(ct))
            .OfType<TypeDeclarationSyntax>()
            .Select(d => d.Keyword.GetLocation());

        foreach (var location in locations)
        {
            ct.ThrowIfCancellationRequested();

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UnionMayNotBeRecordType,
                location,
                messageArgs:
                [
                    attributeContext.TargetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                ]);
            ctx.ReportDiagnostic(diagnostic);
        }
    }
}
