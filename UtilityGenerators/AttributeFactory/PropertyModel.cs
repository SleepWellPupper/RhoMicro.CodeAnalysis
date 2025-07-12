// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;
using RhoMicro.CodeAnalysis.Library.Models.Collections;

internal sealed partial record PropertyModel(
    String Name,
    String? DefaultValueExpression,
    AttributeParameterTypeModel Type,
    Boolean HasSetter,
    EquatableList<ParameterMapping> Mappings)
{
    [TypeSymbolPattern(typeof(String[]))]
    private static partial Boolean IsStringArray(ITypeSymbol? type);
    public static PropertyModel Create(IPropertySymbol property, LazyEquatableDictionary<String, EquatableList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var name = property.Name;
        var hasSetter = property.SetMethod is { };
        var mappings = propertyMappings[name];
        var type = AttributeParameterTypeModel.Create(property.Type, in ctx);

        String? defaultValueExpression = null;

        foreach(var attribute in property.GetAttributes())
        {
            ctx.ThrowIfCancellationRequested();

            if(attribute.IsDefaultValueAttribute() && attribute.ConstructorArguments is [{ } defaultValue])
            {
                defaultValueExpression = IsStringArray(defaultValue.Type)
                    ? $"[{String.Join(", ", defaultValue.Values.Select(c => c.ToCSharpString()))}]"
                    : defaultValue.ToCSharpString();
            }
        }

        // we do not set mappings list to immutable here, as we do not own it;
        // it is linked to its containing dictionary and managed by our caller

        var result = new PropertyModel(
            Name: name,
            DefaultValueExpression: defaultValueExpression,
            Type: type,
            hasSetter,
            mappings);

        return result;
    }
    public override String ToString() => $"{Name}{{g{( HasSetter ? "/s" : "" )}}}<-[{String.Join(", ", Mappings)}]";
}
