namespace RhoMicro.CodeAnalysis.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models.Collections;
using RhoMicro.CodeAnalysis.Library.Text;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed record TypeSignatureModel(
    EquatableList<String> NamespaceParts,
    EquatableList<ContainingTypeSignatureModel> ContainingTypes,
    PartialTypeKindModel Kind,
    String Name,
    EquatableList<String> TypeArguments)
{
    private readonly record struct AppendTokenInfo(String SeparatorToken, String OpenToken, String ArgumentSeparatorToken, String CloseToken, Boolean SeparateParts);
    public static TypeSignatureModel Create(INamedTypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var mutabilityContext = new MutabilityContext();

        var namespaceParts = ctx.CollectionFactory.CreateList<String>(mutabilityContext);
        var containingTypes = ctx.CollectionFactory.CreateList<ContainingTypeSignatureModel>(mutabilityContext);
        var typeArguments = ctx.CollectionFactory.CreateList<String>(mutabilityContext);

        AddNamespaceParts(type.ContainingNamespace, namespaceParts, in ctx);
        AddContainingTypes(type.ContainingType, containingTypes, in ctx);

        foreach(var typeArgument in type.TypeArguments)
        {
            ctx.ThrowIfCancellationRequested();
            typeArguments.Add(typeArgument.Name);
        }

        var kind = PartialTypeKindModel.Create(type, in ctx);
        var name = type.Name;

        mutabilityContext.SetImmutable();

        var result = new TypeSignatureModel(namespaceParts, containingTypes, kind, name, typeArguments);

        return result;
    }

    private static void AddNamespaceParts(INamespaceSymbol? @namespace, EquatableList<String> parts, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(@namespace is null or { IsGlobalNamespace: true })
            return;

        AddNamespaceParts(@namespace.ContainingNamespace, parts, in ctx);

        parts.Add(@namespace.Name);
    }
    private static void AddContainingTypes(INamedTypeSymbol? containingType, EquatableList<ContainingTypeSignatureModel> containingTypes, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        if(containingType is null)
            return;

        AddContainingTypes(containingType.ContainingType, containingTypes, in ctx);

        var model = ContainingTypeSignatureModel.Create(containingType, in ctx);

        containingTypes.Add(model);
    }

    public String GetHintName(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var resultBuilder = new StringBuilder();

        Append(resultBuilder, new(
            SeparatorToken: "_",
            OpenToken: "of_",
            ArgumentSeparatorToken: "_and_",
            CloseToken: "_fo",
            SeparateParts: true), ct);

        var result = resultBuilder.Append(".g.cs").ToString();

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
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);

        AppendName(resultBuilder, Name, ct);
        AppendTypeArguments(resultBuilder, TypeArguments, in tokenInfo, ct);
    }
    private void AppendNamespace(StringBuilder resultBuilder, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(NamespaceParts.Count <= 0)
            return;

        for(var i = 0; i < NamespaceParts.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
                _ = resultBuilder.Append(tokenInfo.SeparatorToken);

            _ = resultBuilder.Append((String?)NamespaceParts[i]);
        }

        if(tokenInfo.SeparateParts)
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
    }
    private void AppendContainingTypes(StringBuilder resultBuilder, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(ContainingTypes.Count == 0)
            return;

        for(var i = 0; i < ContainingTypes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
                _ = resultBuilder.Append(tokenInfo.SeparatorToken);

            var containingType = ContainingTypes[i];

            AppendName(resultBuilder, containingType.Name, ct);
            AppendTypeArguments(resultBuilder, containingType.TypeArguments, tokenInfo, ct);
        }

        if(tokenInfo.SeparateParts)
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);
    }
    private static void AppendName(
        StringBuilder resultBuilder,
        String name,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        _ = resultBuilder.Append(name);
    }
    private static void AppendTypeArguments(StringBuilder resultBuilder, EquatableList<String> typeArguments, in AppendTokenInfo tokenInfo, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(typeArguments.Count == 0)
            return;

        if(tokenInfo.SeparateParts)
            _ = resultBuilder.Append(tokenInfo.SeparatorToken);

        _ = resultBuilder.Append(tokenInfo.OpenToken);

        for(var i = 0; i < typeArguments.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            if(i != 0)
                _ = resultBuilder.Append(tokenInfo.ArgumentSeparatorToken);

            var typeParameter = typeArguments[i];
            _ = resultBuilder.Append(typeParameter);
        }

        _ = resultBuilder.Append(tokenInfo.CloseToken);
    }

    public void BuildStrings(
        IndentedStringBuilder sourceBuilder,
        out String hintName,
        out String displayString,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var hintNameBuilder = new StringBuilder();
        var displayStringBuilder = new StringBuilder("global::");

        Append(
            hintNameBuilder: hintNameBuilder,
            displayStringBuilder: displayStringBuilder,
            sourceBuilder,
            ct);

        hintName = hintNameBuilder.Append(".g.cs").ToString();
        displayString = displayStringBuilder.ToString();
    }
    private void Append(
        StringBuilder hintNameBuilder,
        StringBuilder displayStringBuilder,
        IndentedStringBuilder sourceBuilder,
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
            TypeArguments, ct);

        _ = sourceBuilder.OpenBracesBlock();
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
            return;

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
            return;

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
    private static void AppendTypeArguments(StringBuilder hintNameBuilder, StringBuilder displayStringBuilder, IndentedStringBuilder sourceBuilder, EquatableList<String> typeArguments, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if(typeArguments.Count == 0)
            return;

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

            var typeParameter = typeArguments[i];
            _ = hintNameBuilder.Append('_').Append(typeParameter);
            _ = displayStringBuilder.Append(typeParameter);
            _ = sourceBuilder.Append(typeParameter);
        }

        _ = hintNameBuilder.Append("_fo");
        _ = displayStringBuilder.Append('>');
        _ = sourceBuilder.Append('>');
    }
}
