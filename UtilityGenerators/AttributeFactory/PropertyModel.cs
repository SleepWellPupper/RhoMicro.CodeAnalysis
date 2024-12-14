namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;

internal sealed record PropertyModel(
    String Name,
    String? DefaultValueExpression,
    AttributeParameterTypeModel Type,
    Boolean HasSetter,
    IList<ParameterMapping> Mappings)
{
    public static PropertyModel Create(IPropertySymbol property, IDictionary<String, IList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
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
                defaultValueExpression = defaultValue.ToCSharpString();
            }
        }

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
