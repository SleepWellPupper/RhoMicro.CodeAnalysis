// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System.Runtime.InteropServices;
using Janus;
using Microsoft.CodeAnalysis;

partial class UnionTypeAttribute
{
    [StructLayout(LayoutKind.Auto)]
    partial record struct Model
    {
        internal static readonly SymbolDisplayFormat TypeDisplayFormat = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.ExpandNullable |
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        [InitializationMethod(StateTypeName = "TypeParameterState")]
        private void Initialize(ITypeParameterSymbol target, CancellationToken ct)
        {
            Name ??= target.Name;
            var name = target.Name;
            var docsId = target.GetDocumentationCommentId() ?? String.Empty;
            Type = target switch
            {
                { HasUnmanagedTypeConstraint: true } =>
                    new(VariantTypeKind.Unmanaged,
                        IsNullable: false,
                        IsInterface: false,
                        Name: name,
                        DocsId: docsId),
                { HasValueTypeConstraint: true } =>
                    new(VariantTypeKind.Value,
                        IsNullable: false,
                        IsInterface: false,
                        Name: name,
                        DocsId: docsId),
                { HasReferenceTypeConstraint: true, HasNotNullConstraint: true } =>
                    new(VariantTypeKind.Reference,
                        IsNullable: false,
                        IsInterface: false,
                        Name: name,
                        DocsId: docsId),
                { HasReferenceTypeConstraint: true, HasNotNullConstraint: false } =>
                    new(VariantTypeKind.Reference,
                        IsNullable: true,
                        IsInterface: false,
                        Name: name,
                        DocsId: docsId),
                _ =>
                    new(VariantTypeKind.Unknown,
                        IsNullable: false,
                        IsInterface: false,
                        Name: name,
                        DocsId: docsId),
            };
        }

        [InitializationMethod(StateTypeName = "TypeArgumentState")]
        private void Initialize(ITypeSymbol variant, CancellationToken ct)
        {
            var isArray = false;

            var extractedVariant = variant;

            if (variant is IArrayTypeSymbol { ElementType: { } elementType })
            {
                isArray = true;
                extractedVariant = elementType;
            }

            var isInterface = extractedVariant.TypeKind is TypeKind.Interface;
            var docsId = extractedVariant.GetDocumentationCommentId() ?? String.Empty;
            var variantName = Name;

            if (extractedVariant is INamedTypeSymbol
                {
                    OriginalDefinition:
                    {
                        SpecialType: SpecialType.System_Nullable_T
                    },
                    TypeArguments: [{ } actualVariant]
                })
            {
                variantName ??= actualVariant.Name;
                var name = actualVariant.ToDisplayString(TypeDisplayFormat);
                Type = new(
                    isArray
                        ? VariantTypeKind.Reference
                        : actualVariant.IsUnmanagedOrUnmanagedUnionType
                            ? VariantTypeKind.Unmanaged
                            : VariantTypeKind.Value,
                    IsNullable: true,
                    IsInterface: isInterface,
                    Name: name,
                    DocsId: docsId);
            }
            else
            {
                variantName ??= extractedVariant.Name;
                var name = variant.ToDisplayString(TypeDisplayFormat);
                Type = extractedVariant switch
                {
                    { IsUnmanagedOrUnmanagedUnionType: true } =>
                        new(isArray
                                ? VariantTypeKind.Reference
                                : VariantTypeKind.Unmanaged,
                            IsNullable: false,
                            IsInterface: isInterface,
                            Name: name,
                            DocsId: docsId),
                    { IsValueType: true } =>
                        new(isArray
                                ? VariantTypeKind.Reference
                                : VariantTypeKind.Value,
                            IsNullable: false,
                            IsInterface: isInterface,
                            Name: name,
                            DocsId: docsId),
                    { IsReferenceType: true } =>
                        new(VariantTypeKind.Reference,
                            IsNullable: IsNullable,
                            IsInterface: isInterface,
                            Name: name,
                            DocsId: docsId),
                    _ =>
                        new(isArray
                                ? VariantTypeKind.Reference
                                : VariantTypeKind.Unknown,
                            IsNullable: false,
                            IsInterface: false,
                            Name: name,
                            DocsId: docsId),
                };
            }

            Name ??= isArray
                ? $"{variantName}Array"
                : variantName;
        }

        public VariantTypeModel Type { get; private set; }
    }

    [DefaultValue((String[])[])]
    public override String[] Groups
    {
        get => base.Groups;
        set => base.Groups = value;
    }

    public override String? Name
    {
        get => base.Name;
        set => base.Name = value;
    }

    public override String? Description
    {
        get => base.Description;
        set => base.Description = value;
    }

    public override Boolean IsNullable
    {
        get => base.IsNullable;
        set => base.IsNullable = value;
    }

    public override Boolean IsDefault
    {
        get => base.IsNullable;
        set => base.IsNullable = value;
    }
}

file static class Extensions
{
    extension(ITypeSymbol symbol)
    {
        public Boolean IsUnmanagedOrUnmanagedUnionType
        {
            get
            {
                var result = isUnmanaged(symbol, null);

                return result;

                Boolean isUnmanaged(ITypeSymbol candidate, Dictionary<ITypeSymbol, Boolean?>? unmanagedTypes)
                {
                    if (!candidate.IsUnmanagedType)
                    {
                        unmanagedTypes?[candidate] = false;
                        return false;
                    }

                    unmanagedTypes = new Dictionary<ITypeSymbol, Boolean?>(SymbolEqualityComparer.Default);

                    if (unmanagedTypes.TryGetValue(candidate, out var memoizedValue))
                    {
                        if (memoizedValue is { } memoizedResult)
                        {
                            return memoizedResult;
                        }

                        return true;
                    }

                    var typeAttributes = candidate.GetAttributes();
                    foreach (var attribute in typeAttributes)
                    {
                        if (!attribute.IsUnionTypeAttribute())
                        {
                            continue;
                        }

                        foreach (var variant in attribute.AttributeClass?.TypeArguments ?? [])
                        {
                            if (isUnmanaged(variant, unmanagedTypes))
                            {
                                continue;
                            }

                            unmanagedTypes[candidate] = false;
                            return false;
                        }
                    }

                    if (candidate is INamedTypeSymbol namedCandidate)
                    {
                        foreach (var typeParameter in namedCandidate.TypeParameters)
                        {
                            if (typeParameter.HasUnmanagedTypeConstraint)
                            {
                                continue;
                            }

                            var typeParameterAttributes = typeParameter.GetAttributes();

                            foreach (var attribute in typeParameterAttributes)
                            {
                                if (attribute.IsUnionTypeAttribute())
                                {
                                    unmanagedTypes[candidate] = false;
                                    return false;
                                }
                            }
                        }
                    }

                    unmanagedTypes[candidate] = true;
                    return true;
                }
            }
        }
    }
}
