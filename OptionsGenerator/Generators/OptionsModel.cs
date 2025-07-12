// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.OptionsGenerator.Generators;

using System;
using System.Diagnostics.CodeAnalysis;

using Analyzers;

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

            if(!OptionsAnalyzer.IsTargetProperty(member, out var p))
                continue;

            if(!OptionsAnalyzer.IsValidTargetProperty(p))
            {
                result = null;
                return false;
            }

            var property = PropertyModel.Create(p, in ctx);
            properties.Add(property);
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