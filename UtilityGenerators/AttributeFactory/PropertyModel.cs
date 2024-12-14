namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using RhoMicro.CodeAnalysis.Library.Models;

internal sealed record PropertyModel(
    String Name,
    String Type,
    String? DefaultValueExpression,
    PropertyTypeKind TypeKind,
    Boolean HasSetter,
    IList<ParameterMapping> Mappings)
{
    public static PropertyModel Create(IPropertySymbol property, IDictionary<String, IList<ParameterMapping>> propertyMappings, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        var name = property.Name;
        var hasSetter = property.SetMethod is { };
        var mappings = propertyMappings[name];
        var typeKind = property.Type switch
        {
            // value type?
            { IsValueType: true } => PropertyTypeKind.ValueType,
            // type?
            { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } } t => t switch
            {
                // nullable type?
                { NullableAnnotation: NullableAnnotation.Annotated } => PropertyTypeKind.NullableType,
                // type
                _ => PropertyTypeKind.Type
            },
            // array?
            IArrayTypeSymbol a => a switch
            {
                // nullable array?
                { NullableAnnotation: NullableAnnotation.Annotated, ElementType: { } et } => et switch
                {
                    // value type nullable array?
                    { IsValueType: true } => PropertyTypeKind.ValueTypeNullableArray,
                    // type nullable array?
                    { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } } => et switch
                    {
                        // nullable type nullable array?
                        { NullableAnnotation: NullableAnnotation.Annotated } => PropertyTypeKind.NullableTypeNullableArray,
                        // type nullable array
                        _ => PropertyTypeKind.TypeNullableArray
                    },
                    // reference type nullable array
                    _ => et switch
                    {
                        // nullable reference type nullable array?
                        { NullableAnnotation: NullableAnnotation.Annotated } => PropertyTypeKind.NullableReferenceTypeNullableArray,
                        // reference type nullable array
                        _ => PropertyTypeKind.ReferenceTypeNullableArray
                    }
                },
                // array
                _ => a switch
                {
                    // value type array?
                    { IsValueType: true } => PropertyTypeKind.ValueTypeArray,
                    // type array?
                    { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } } => PropertyTypeKind.TypeArray,
                    // reference type array
                    _ => PropertyTypeKind.ReferenceTypeArray
                }
            },
            // reference type
            { } r => r switch
            {
                // nullable reference type?
                { NullableAnnotation: NullableAnnotation.Annotated } => PropertyTypeKind.NullableReferenceType,
                // reference type
                _ => PropertyTypeKind.ReferenceType
            }
        };
        var type = property.Type is IArrayTypeSymbol { ElementType: { } elementType }
            ? $"global::System.Collections.Immutable.ImmutableArray<{elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>"
            : property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

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
            Type: type,
            DefaultValueExpression: defaultValueExpression,
            typeKind,
            hasSetter,
            mappings);

        return result;
    }
    public override String ToString() => $"{Name}{{g{( HasSetter ? "/s" : "" )}}}<-[{String.Join(", ", Mappings)}]";
}
