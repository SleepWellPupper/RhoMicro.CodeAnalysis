// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Library.Models;
using Library.Models.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal sealed partial record UnionModel(
    UnionTypeKind TypeKind,
    String Name,
    String Namespace,
    EquatableList<ContainingTypeModel> ContainingTypes,
    EquatableList<UnionTypeAttribute.Model> Variants,
    EquatableList<String> VariantGroups,
    EquatableList<String> TypeParameters,
    UnionTypeSettingsAttribute.Model Settings,
    Boolean IsToStringUserProvided,
    Boolean IsEqualsUserProvided,
    Boolean IsGetHashCodeUserProvided,
    Boolean AreEqualityOperatorsUserProvided,
    String DocsCommentId,
    Boolean EmitDocsComment)
{
    public Boolean HasValueTypeVariant => Variants is
    [
        { Type.Kind: VariantTypeKind.Unmanaged or VariantTypeKind.Value },
        ..
    ];

    [field: MaybeNull] public TypeNames TypeNames => field ??= new TypeNames(this);

    private static readonly SymbolDisplayFormat _namespaceFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    private static Boolean HasUnsupportedStateSettingsUnaware(INamedTypeSymbol target, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // type keywords would be incorrectly emitted
        if (target.IsRecord)
        {
            return true;
        }

        // partial class would not be emitted as static
        if (target.IsStatic)
        {
            return true;
        }

        // ref structs introduce (for now) unsupported complexities
        if (target.IsRefLikeType)
        {
            return true;
        }

        return false;
    }

    public UnionModelValidation GetValidation(CancellationToken ct) => new(this, ct);

    public static Boolean TryCreate(
        INamedTypeSymbol target,
        [NotNullWhen(true)] out UnionModel? model,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (HasUnsupportedStateSettingsUnaware(target, ct))
        {
            model = null;
            return false;
        }

        model = Create(target, ct);

        if (model.GetValidation(ct).HasUnsupportedSettingsUnawareStatePostCondition(target))
        {
            model = null;
            return false;
        }

        return true;
    }

    public static UnionModel Create(
        INamedTypeSymbol target,
        CancellationToken ct)
    {
        using var ctx = ModelCreationContext.CreateDefault(ct);

        var docsCommentId = target.GetDocumentationCommentId() ?? String.Empty;

        var variantsList = new List<UnionTypeAttribute.Model>();
        var variantGroupsList = new List<String>();
        var typeParameters = ctx.CollectionFactory.CreateList<String>();
        ParseTargetAttributes(
            target,
            variantsList,
            variantGroupsList,
            out var settings,
            ct);
        ParseTargetTypeParametersAttributes(
            target: target,
            variants: variantsList,
            variantGroups: variantGroupsList,
            typeParameters: typeParameters, ct: ct);

        var variants = SortVariants(variantsList, ctx);
        var variantGroups = SortVariantGroups(variantGroupsList, ctx);

        var typeKind = target.TypeKind is Microsoft.CodeAnalysis.TypeKind.Struct
            ? UnionTypeKind.Struct
            : UnionTypeKind.Class;
        var containingTypes = ctx.CollectionFactory.CreateList<ContainingTypeModel>();
        if (target.ContainingType is { } t)
        {
            AppendContainingType(t, containingTypes, in ctx);
        }

        var (isToStringUserProvided,
            isEqualsUserProvided,
            isGetHashCodeUserProvided,
            areEqualityOperatorsUserProvided) = (false, false, false, false);
        var members = target.GetMembers();

        foreach (var member in members)
        {
            ct.ThrowIfCancellationRequested();

            switch (member.Name)
            {
                case nameof(ToString):
                    if (member is IMethodSymbol { Parameters: [] })
                    {
                        isToStringUserProvided = true;
                    }

                    break;
                case nameof(Equals):
                    if (member is IMethodSymbol { Parameters: [{ } singleParameter] }
                     && SymbolEqualityComparer.Default.Equals(singleParameter.Type, target))
                    {
                        isEqualsUserProvided = true;
                    }

                    break;
                case nameof(GetHashCode):
                    if (member is IMethodSymbol { Parameters: [] })
                    {
                        isGetHashCodeUserProvided = true;
                    }

                    break;
                case "op_Equality" or "op_Inequality":
                    if (member is IMethodSymbol { Parameters: [{ } firstParameter, { } secondParameter] }
                     && SymbolEqualityComparer.Default.Equals(firstParameter.Type, target)
                     && SymbolEqualityComparer.Default.Equals(secondParameter.Type, target))
                    {
                        areEqualityOperatorsUserProvided = true;
                    }

                    break;
            }
        }

        var emitDocsComment = target.GetDocumentationCommentXml(cancellationToken: ct) is null or [];

        var result = new UnionModel(
            TypeKind: typeKind,
            Name: target.Name,
            Namespace: target.ContainingNamespace.ToDisplayString(_namespaceFormat),
            ContainingTypes: containingTypes,
            Variants: variants,
            VariantGroups: variantGroups,
            TypeParameters: typeParameters,
            Settings: settings,
            IsToStringUserProvided: isToStringUserProvided,
            IsEqualsUserProvided: isEqualsUserProvided,
            IsGetHashCodeUserProvided: isGetHashCodeUserProvided,
            AreEqualityOperatorsUserProvided: areEqualityOperatorsUserProvided,
            DocsCommentId: docsCommentId,
            EmitDocsComment: emitDocsComment
        );

        return result;
    }

    private static EquatableList<String> SortVariantGroups(
        List<String> variantGroupsList,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        variantGroupsList.Sort((x, y) => x.CompareTo(y, StringComparison.Ordinal));
        variantGroupsList.Insert(0, "None");
        var variantGroups = ctx.CollectionFactory.CreateList<String>();
        foreach (var variantGroup in variantGroupsList)
        {
            ctx.ThrowIfCancellationRequested();

            variantGroups.Add(variantGroup);
        }

        return variantGroups;
    }

    private static EquatableList<UnionTypeAttribute.Model> SortVariants(
        List<UnionTypeAttribute.Model> variantsList,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        variantsList.Sort((x, y) =>
        {
            var xTypePrecedence = getTypePrecedence(x.Type);
            var yTypePrecedence = getTypePrecedence(y.Type);
            // negative order means x precedes y in sort order, x has higher precedence than y
            // positive order means y precedes x in sort order, y has higher precedence than x
            var typeOrder = yTypePrecedence - xTypePrecedence;

            var result = typeOrder == 0
                ? x.Name.CompareTo(y.Name, StringComparison.Ordinal)
                : typeOrder;

            return result;

            static Int32 getTypePrecedence(VariantTypeModel type)
            {
                var result = type switch
                {
                    { Kind: VariantTypeKind.Unmanaged } => 3,
                    { Kind: VariantTypeKind.Value } => 2,
                    { Kind: VariantTypeKind.Reference or VariantTypeKind.Unknown, IsNullable: true } => 1,
                    _ => 0
                };

                return result;
            }
        });
        var variants = ctx.CollectionFactory.CreateList<UnionTypeAttribute.Model>();
        foreach (var variant in variantsList)
        {
            ctx.ThrowIfCancellationRequested();

            variants.Add(variant);
        }

        return variants;
    }

    private static void AppendContainingType(
        INamedTypeSymbol type,
        EquatableList<ContainingTypeModel> containingTypes,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if (type.ContainingType is { } t)
        {
            AppendContainingType(t, containingTypes, in ctx);
        }

        var typeParameters = ctx.CollectionFactory.CreateList<String>();

        for (var i = 0; i < type.TypeParameters.Length; i++)
        {
            ctx.ThrowIfCancellationRequested();

            typeParameters.Add(type.TypeParameters[i].Name);
        }

        var containingType = new ContainingTypeModel(
            $"{(type.IsRecord ? "record " : String.Empty)}{(type.TypeKind is Microsoft.CodeAnalysis.TypeKind.Struct ? "struct" : "class")}",
            type.Name,
            typeParameters);

        containingTypes.Add(containingType);
    }

    private static void ParseTargetTypeParametersAttributes(
        INamedTypeSymbol target,
        List<UnionTypeAttribute.Model> variants,
        List<String> variantGroups,
        EquatableList<String> typeParameters,
        CancellationToken ct)
    {
        foreach (var typeParameter in target.TypeParameters)
        {
            ct.ThrowIfCancellationRequested();

            typeParameters.Add(typeParameter.Name);

            var attributes = typeParameter.GetAttributes();
            foreach (var attributeData in attributes)
            {
                ct.ThrowIfCancellationRequested();

                if (!attributeData.TryGetUnionTypeAttributeModel(
                    new UnionTypeAttribute.Model.TypeParameterState(typeParameter),
                    out var variant,
                    cancellationToken: ct))
                {
                    continue;
                }

                variants.Add(variant);

                AddDistinctVariantGroups(variant, variantGroups, ct);
            }
        }
    }

    private static void ParseTargetAttributes(
        INamedTypeSymbol target,
        List<UnionTypeAttribute.Model> variants,
        List<String> variantGroups,
        out UnionTypeSettingsAttribute.Model settings,
        CancellationToken ct)
    {
        settings = UnionTypeSettingsAttribute.Model.Default;

        foreach (var attributeData in target.GetAttributes())
        {
            ct.ThrowIfCancellationRequested();

            if (attributeData.TryGetUnionTypeSettingsAttributeModel(out var s, cancellationToken: ct))
            {
                settings = s;
                continue;
            }

            if (attributeData.AttributeClass?.TypeArguments is not [_, ..] typeArguments)
            {
                continue;
            }

            foreach (var typeArgument in typeArguments)
            {
                ct.ThrowIfCancellationRequested();

                if (!attributeData.TryGetUnionTypeAttributeModel(
                    new UnionTypeAttribute.Model.TypeArgumentState(typeArgument),
                    out var variant,
                    cancellationToken: ct))
                {
                    continue;
                }

                variants.Add(variant);

                AddDistinctVariantGroups(variant, variantGroups, ct);
            }
        }
    }

    private static void AddDistinctVariantGroups(
        UnionTypeAttribute.Model variant, List<String> variantGroups,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        foreach (var group in variant.Groups)
        {
            ct.ThrowIfCancellationRequested();

            if (!variantGroups.Contains(group))
            {
                variantGroups.Add(group);
            }
        }
    }
}
