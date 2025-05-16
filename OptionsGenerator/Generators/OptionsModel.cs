namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed record OptionsModel(
    EquatableList<PropertyModel> Properties,
    String Namespace,
    String Name,
    String FullyQualifiedNamespacePrefix,
    String NamespacePrefix,
    String NormalizedName)
{
    public static Boolean TryCreate(
        INamedTypeSymbol type,
        OptionsAttribute.Model _,
        [NotNullWhen(true)] out OptionsModel? result,
        in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var properties = ctx.CollectionFactory.CreateList<PropertyModel>();

        foreach(var member in type.GetMembers())
        {
            ctx.ThrowIfCancellationRequested();

            if(member is IPropertySymbol
               {
                   SetMethod:not null
               } && !member.GetAttributes().Any(a=>a.IsExcludeFromOptionsAttribute()))
            {
                result = null;
                return false;
            }
            
            if(PropertyModel.TryCreate(member, out var p, in ctx))
                properties.Add(p);
        }

        var @namespace = type.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.GlobalOmittedNamespaceFormat);

        result = new OptionsModel(
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

        return true;
    }

    public Templates Templates() => new(this);
}
