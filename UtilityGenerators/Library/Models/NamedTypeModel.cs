// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models;
using System;
using System.Text;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text.SourceTexts;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed record NamedTypeModel(
    EquatableList<ContainingTypeSignatureModel> ContainingTypes,
    Accessibility? Accessibility,
    PartialTypeKindModel Kind,
    EquatableList<TypeModel> TypeArguments,
    EquatableList<String> NamespaceParts,
    String Name) : TypeModel(
        NamespaceParts,
        Name)
{
    private readonly record struct AppendTokenInfo(String SeparatorToken, String OpenToken, String ArgumentSeparatorToken, String CloseToken, Boolean SeparateParts);

    public static NamedTypeModel Create(INamedTypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var namespaceParts = GetNamespaceParts(type, in ctx);
        var containingTypes = ctx.CollectionFactory.CreateList<ContainingTypeSignatureModel>();
        var typeArguments = ctx.CollectionFactory.CreateList<TypeModel>();

        AddContainingTypes(type.ContainingType, containingTypes, in ctx);

        foreach(var typeArgument in type.TypeArguments)
        {
            ctx.ThrowIfCancellationRequested();
            var model = Create(typeArgument, in ctx);
            typeArguments.Add(model);
        }

        var accessibility = type.DeclaredAccessibility;
        var kind = PartialTypeKindModel.Create(type, in ctx);
        var name = type.Name;

        var result = new NamedTypeModel(
            containingTypes,
            accessibility,
            kind,
            typeArguments,
            namespaceParts,
            name);

        return result;
    }

    private static void AddContainingTypes(INamedTypeSymbol? containingType, EquatableList<ContainingTypeSignatureModel> containingTypes, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(containingType is null)
        {
            return;
        }

        AddContainingTypes(containingType.ContainingType, containingTypes, in ctx);

        var model = ContainingTypeSignatureModel.Create(containingType, in ctx);

        containingTypes.Add(model);
    }

    public String GetHintName(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var resultBuilder = new StringBuilder();

        foreach(var part in NamespaceParts)
        {
            _ = resultBuilder.Append(part).Append('.');
        }

        foreach(var containingType in ContainingTypes)
        {
            _ = resultBuilder.Append(containingType.Name).Append('.');

            if(containingType.TypeArguments is { Count: > 0 and var innerCount })
            {
                _ = resultBuilder.Append(innerCount).Append('.');
            }
        }

        _ = resultBuilder.Append(Name).Append('.');

        if(TypeArguments is { Count: > 0 and var outerCount })
        {
            _ = resultBuilder.Append(outerCount).Append('.');
        }

        var result = resultBuilder.Append("g.cs").ToString();

        return result;
    }
    public String GetDisplayString(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var resultBuilder = new StringBuilder();

        Append(resultBuilder, new(
            SeparatorToken: ".",
            OpenToken: "<",
            ArgumentSeparatorToken: ", ",
            CloseToken: ">",
            SeparateParts: false), ct);

        var result = resultBuilder.ToString();

        return result;
    }
    private void Append(StringBuilder resultBuilder, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        AppendNamespace(resultBuilder, in tokenInfo, ct);
        AppendContainingTypes(resultBuilder, in tokenInfo, ct);

        if(!tokenInfo.SeparateParts && ( ContainingTypes.Count > 0 || NamespaceParts.Count > 0 ))
        {
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
        }

        AppendName(resultBuilder, Name, ct);
        AppendTypeArguments(resultBuilder, TypeArguments, in tokenInfo, ct);
    }
    private void AppendNamespace(StringBuilder resultBuilder, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(NamespaceParts.Count <= 0)
        {
            return;
        }

        for(var i = 0; i < NamespaceParts.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                _ = resultBuilder.Append(tokenInfo.SeparatorToken);
            }

            _ = resultBuilder.Append((String?)NamespaceParts[i]);
        }

        if(tokenInfo.SeparateParts)
        {
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
        }
    }
    private void AppendContainingTypes(StringBuilder resultBuilder, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(ContainingTypes.Count == 0)
        {
            return;
        }

        for(var i = 0; i < ContainingTypes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                _ = resultBuilder.Append(tokenInfo.SeparatorToken);
            }

            var containingType = ContainingTypes[i];

            AppendName(resultBuilder, containingType.Name, ct);
            AppendTypeArguments(resultBuilder, containingType.TypeArguments, tokenInfo, ct);
        }

        if(tokenInfo.SeparateParts)
        {
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
        }
    }
    private static void AppendName(
        StringBuilder resultBuilder,
        String name,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        _ = resultBuilder.Append(name);
    }
    private static void AppendTypeArguments(StringBuilder resultBuilder, EquatableList<TypeModel> typeArguments, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(typeArguments.Count == 0)
        {
            return;
        }

        if(tokenInfo.SeparateParts)
        {
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
        }

        _ = resultBuilder.Append(tokenInfo.OpenToken);

        for(var i = 0; i < typeArguments.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                _ = resultBuilder.Append(tokenInfo.ArgumentSeparatorToken);
            }

            var typeArgument = typeArguments[i];
            _ = resultBuilder.Append(typeArgument.Name);
        }

        _ = resultBuilder.Append(tokenInfo.CloseToken);
    }

    public void BuildStrings(
        IndentedStringBuilder sourceBuilder,
        out String hintName,
        out String displayString,
        CancellationToken ct) =>
        BuildStrings(sourceBuilder, out hintName, out displayString, [], ct);
    public void BuildStrings(
        IndentedStringBuilder sourceBuilder,
        out String hintName,
        out String displayString,
        ReadOnlySpan<String> superTypes,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var hintNameBuilder = new StringBuilder();
        var displayStringBuilder = new StringBuilder("global::");

        Append(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            superTypes,
            ct);

        hintName = hintNameBuilder.Append(".g.cs").ToString();
        displayString = displayStringBuilder.ToString();
    }
    private void Append(
        StringBuilder hintNameBuilder,
        StringBuilder displayStringBuilder,
        IndentedStringBuilder sourceBuilder,
        ReadOnlySpan<String> superTypes,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        AppendNamespace(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            ct);

        AppendContainingTypes(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            ct);

        AppendName(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            Name,
            Kind,
            ct);

        AppendTypeArguments(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            TypeArguments,
            ct);

        AppendImplementedInterfaces(
            sourceBuilder,
            superTypes,
            ct);

        _ = sourceBuilder.OpenBracesBlock();
    }
    private static void AppendImplementedInterfaces(
        IndentedStringBuilder sourceBuilder,
        ReadOnlySpan<String> superTypes,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(superTypes.Length == 0)
        {
            return;
        }

        using var _ = sourceBuilder.Append(" :").AppendLine().OpenIndentBlockScope();

        for(var i = 0; i < superTypes.Length; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                sourceBuilder.Append(',').AppendLineCore();
            }

            sourceBuilder.AppendCore(superTypes[i]);
        }
    }
    private static void AppendName(
        StringBuilder hintNameBuilder,
        StringBuilder displayStringBuilder,
        IndentedStringBuilder sourceBuilder,
        String name,
        PartialTypeKindModel kind,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        _ = hintNameBuilder.Append(name);
        _ = displayStringBuilder.Append(name);
        _ = sourceBuilder.Append("partial ").Append(kind.Value).Append(' ').Append(name);
    }
    private void AppendNamespace(
        StringBuilder hintNameBuilder,
        StringBuilder displayStringBuilder,
        IndentedStringBuilder sourceBuilder,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(NamespaceParts.Count <= 0)
        {
            return;
        }

        _ = sourceBuilder.Append("namespace ");

        for(var i = 0; i < NamespaceParts.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                _ = hintNameBuilder.Append('_');
                _ = displayStringBuilder.Append('.');
                _ = sourceBuilder.Append('.');
            }

            var part = NamespaceParts[i];
            _ = hintNameBuilder.Append(part);
            _ = displayStringBuilder.Append(part);
            _ = sourceBuilder.Append(part);
        }

        _ = displayStringBuilder.Append('.');
        _ = hintNameBuilder.Append('_');
        _ = sourceBuilder.AppendLine(';');
    }
    private void AppendContainingTypes(StringBuilder hintNameBuilder, StringBuilder displayStringBuilder, IndentedStringBuilder sourceBuilder, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(ContainingTypes.Count == 0)
        {
            return;
        }

        for(var i = 0; i < ContainingTypes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var containingType = ContainingTypes[i];

            AppendName(
                hintNameBuilder: hintNameBuilder,
                displayStringBuilder: displayStringBuilder,
                sourceBuilder,
                containingType.Name,
                containingType.Kind,
                ct);

            AppendTypeArguments(
                hintNameBuilder: hintNameBuilder,
                displayStringBuilder: displayStringBuilder,
                sourceBuilder,
                containingType.TypeArguments,
                ct);

            _ = sourceBuilder.OpenBracesBlock();
            _ = hintNameBuilder.Append('_');
            _ = displayStringBuilder.Append('.');
        }
    }
    private static void AppendTypeArguments(StringBuilder hintNameBuilder, StringBuilder displayStringBuilder, IndentedStringBuilder sourceBuilder, EquatableList<TypeModel> typeArguments, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(typeArguments.Count == 0)
        {
            return;
        }

        _ = hintNameBuilder.Append("_of");
        _ = displayStringBuilder.Append('<');
        _ = sourceBuilder.Append('<');

        for(var i = 0; i < typeArguments.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
            {
                _ = hintNameBuilder.Append("_and");
                _ = displayStringBuilder.Append(", ");
                _ = sourceBuilder.Append(", ");
            }

            var typeArgument = typeArguments[i];
            _ = hintNameBuilder.Append('_').Append(typeArgument.Name);
            _ = displayStringBuilder.Append(typeArgument.Name);
            _ = sourceBuilder.Append(typeArgument.Name);
        }

        _ = hintNameBuilder.Append("_fo");
        _ = displayStringBuilder.Append('>');
        _ = sourceBuilder.Append('>');
    }
}
