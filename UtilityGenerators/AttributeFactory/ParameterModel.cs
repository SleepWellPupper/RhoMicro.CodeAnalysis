namespace RhoMicro.CodeAnalysis;

using System;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis.Library.Models;
internal sealed record ParameterModel(
    String Name,
    Int32 Index,
    AttributeParameterTypeModel Type,
    String? MappedProperty,
    String Pattern)
{
    public static ParameterModel Create(IParameterSymbol parameter, Int32 index, in ModelCreationContext ctx)
    {
        // we only support explicit mapping for now
        // ctor param mapping resolution:
        // - attribute (duplicates: last declared wins)
        // TODO later (maybe)
        // - assignment to prop in ctor
        // - call to this(): transitive map => memoize to avoid SOE
        // - constant value assignment to property

        ctx.ThrowIfCancellationRequested();

        var pattern = parameter.Type is IArrayTypeSymbol { ElementType: { } elementType }
            ? $"{{ Type: global::Microsoft.CodeAnalysis.IArrayTypeSymbol {{ ElementType: {getNonArrayPattern(elementType)} }} }}"
            : $"{{ Type: {getNonArrayPattern(parameter.Type)} }}";
        var name = parameter.Name;
        var type = AttributeParameterTypeModel.Create(parameter.Type, in ctx);
        String? mappedProperty = null;

        foreach(var attribute in parameter.GetAttributes())
        {
            ctx.ThrowIfCancellationRequested();

            if(attribute.GetMapToPropertyAttributeConstructorArgumentAccessor().TryGetPropertyName(out var propertyName))
            {
                mappedProperty = propertyName;
            }
        }

        var result = new ParameterModel(
            Name: name,
            index,
            type,
            MappedProperty: mappedProperty,
            Pattern: pattern);

        return result;

        static String getNonArrayPattern(ITypeSymbol type) =>
            type switch
            {
                { SpecialType: SpecialType.System_Boolean } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Boolean }",
                { SpecialType: SpecialType.System_String } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_String }",
                { SpecialType: SpecialType.System_Single } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Single }",
                { SpecialType: SpecialType.System_Double } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Double }",
                { SpecialType: SpecialType.System_SByte } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_SByte }",
                { SpecialType: SpecialType.System_Int16 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Int16 }",
                { SpecialType: SpecialType.System_Int32 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Int32 }",
                { SpecialType: SpecialType.System_Int64 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Int64 }",
                { SpecialType: SpecialType.System_Byte } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Byte }",
                { SpecialType: SpecialType.System_UInt16 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_UInt16 }",
                { SpecialType: SpecialType.System_UInt32 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_UInt32 }",
                { SpecialType: SpecialType.System_UInt64 } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_UInt64 }",
                { SpecialType: SpecialType.System_Object } =>
                    "{ SpecialType: global::Microsoft.CodeAnalysis.SpecialType.System_Object }",
                { Name: "Type", ContainingType: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } =>
                    """{ Name: "Type", ContainingType: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } }""",
                _ => "{ }"
            };
    }

    public override String ToString() => $"{Index}: {Name}->{MappedProperty ?? "?"}";
}
