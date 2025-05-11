namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record OptionsInterfaceModel(
    EquatableList<PropertyModel> Properties,
    String Namespace,
    String Name,
    String FullyQualifiedNamespacePrefix,
    String NamespacePrefix,
    String NormalizedName)
{
    public static OptionsInterfaceModel Create(
        INamedTypeSymbol type,
        OptionsAttribute.Model _,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var properties = ctx.CollectionFactory.CreateList<PropertyModel>();

        foreach(var member in type.GetMembers())
        {
            ctx.ThrowIfCancellationRequested();

            if(PropertyModel.TryCreate(member, out var p, in ctx))
                properties.Add(p);
        }

        var @namespace = type.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.GlobalOmittedNamespaceFormat);

        var result = new OptionsInterfaceModel(
            Properties: properties,
            Namespace: @namespace,
            FullyQualifiedNamespacePrefix: @namespace.Length > 0
                ? $"global::{@namespace}."
                : String.Empty,
            NamespacePrefix: @namespace.Length > 0
                ? $"{@namespace}."
                : String.Empty,
            Name: type.Name,
            NormalizedName: type.Name[1..]);

        return result;
    }
}
