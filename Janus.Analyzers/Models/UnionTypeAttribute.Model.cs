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
        internal static readonly SymbolDisplayFormat TypeDisplayFormat = new SymbolDisplayFormat(
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
            var isInterface = variant.TypeKind is TypeKind.Interface;
            var docsId = variant.GetDocumentationCommentId() ?? String.Empty;

            if (variant is INamedTypeSymbol
                {
                    OriginalDefinition:
                    {
                        SpecialType: SpecialType.System_Nullable_T
                    },
                    TypeArguments: [{ } actualVariant]
                })
            {
                Name ??= actualVariant.Name;
                var name = actualVariant.ToDisplayString(TypeDisplayFormat);
                Type = new(
                    actualVariant.IsUnmanagedType ? VariantTypeKind.Unmanaged : VariantTypeKind.Value,
                    IsNullable: true,
                    IsInterface: isInterface,
                    Name: name,
                    DocsId: docsId);
            }
            else
            {
                Name ??= variant.Name;
                var name = variant.ToDisplayString(TypeDisplayFormat);
                Type = variant switch
                {
                    { IsUnmanagedType: true } =>
                        new(VariantTypeKind.Unmanaged,
                            IsNullable: false,
                            IsInterface: isInterface,
                            Name: name,
                            DocsId: docsId),
                    { IsValueType: true } =>
                        new(VariantTypeKind.Value,
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
                        new(VariantTypeKind.Unknown,
                            IsNullable: false,
                            IsInterface: false,
                            Name: name,
                            DocsId: docsId),
                };
            }
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
}
