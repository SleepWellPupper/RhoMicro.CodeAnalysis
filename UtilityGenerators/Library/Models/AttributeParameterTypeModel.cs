namespace RhoMicro.CodeAnalysis.Library.Models;
using System;

using Microsoft.CodeAnalysis;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal sealed record AttributeParameterTypeModel(
    AttributeParameterTypeKind Kind,
    String KindString,
    String DisplayString,
    String ElementDisplayString,
    String NullableDisplayString)
{
    private const String _typeSymbolDisplayString = "global::Microsoft.CodeAnalysis.ITypeSymbol";
    private const String _nullableTypeSymbolDisplayString = "global::Microsoft.CodeAnalysis.ITypeSymbol?";

    public static AttributeParameterTypeModel Create(ITypeSymbol type, in ModelCreationContext ctx)
    {
        ctx.ThrowIfCancellationRequested();

        AttributeParameterTypeKind kind;
        String displayString;
        String elementDisplayString;

        var isTypeNullableAnnotated = type.NullableAnnotation == NullableAnnotation.Annotated;

        // value type?
        if(type.IsValueType)
        {
            // enum?
            kind = type.BaseType is { SpecialType: SpecialType.System_Enum }
                ? AttributeParameterTypeKind.Enum
                : AttributeParameterTypeKind.ValueType;

            displayString = elementDisplayString = ToDisplayString(type);
        }
        // type?
        else if(IsType(type))
        {
            // nullable type?
            if(isTypeNullableAnnotated)
            {
                kind = AttributeParameterTypeKind.NullableType;
                displayString = elementDisplayString = _nullableTypeSymbolDisplayString;
            }
            // type
            else
            {
                kind = AttributeParameterTypeKind.Type;
                displayString = elementDisplayString = _typeSymbolDisplayString;
            }
        }
        // array?
        else if(type is IArrayTypeSymbol array)
        {
            var elementType = array.ElementType;
            var isElementTypeNullableAnnotated = elementType.NullableAnnotation == NullableAnnotation.Annotated;

            // nullable array?
            if(isTypeNullableAnnotated)
            {
                // value type nullable array?
                if(elementType.IsValueType)
                {
                    // enum nullable array?
                    kind = elementType.BaseType is { SpecialType: SpecialType.System_Enum }
                        ? AttributeParameterTypeKind.EnumNullableArray
                        : AttributeParameterTypeKind.ValueTypeNullableArray;

                    displayString = ToArrayDisplayString(elementType, nullable: true);
                    elementDisplayString = ToDisplayString(elementType);
                }
                // type nullable array?
                else if(IsType(elementType))
                {
                    // nullable type nullable array?
                    if(isElementTypeNullableAnnotated)
                    {
                        kind = AttributeParameterTypeKind.NullableTypeNullableArray;
                        displayString = ToArrayDisplayString(_nullableTypeSymbolDisplayString, nullable: true);
                        elementDisplayString = _nullableTypeSymbolDisplayString;
                    }
                    // type nullable array
                    else
                    {
                        kind = AttributeParameterTypeKind.TypeNullableArray;
                        displayString = ToArrayDisplayString(_typeSymbolDisplayString, nullable: true);
                        elementDisplayString = _typeSymbolDisplayString;
                    }
                }
                // reference type nullable array
                else
                {
                    // nullable reference type nullable array?
                    if(isElementTypeNullableAnnotated)
                    {
                        kind = AttributeParameterTypeKind.NullableReferenceTypeNullableArray;
                        displayString = ToArrayDisplayString(elementType, nullable: true);
                        elementDisplayString = ToDisplayString(elementType);
                    }
                    // reference type nullable array
                    else
                    {
                        kind = AttributeParameterTypeKind.ReferenceTypeNullableArray;
                        displayString = ToArrayDisplayString(elementType, nullable: true);
                        elementDisplayString = ToDisplayString(elementType);
                    }
                }
            }
            // array
            else
            {
                // value type array?
                if(elementType.IsValueType)
                {
                    // enum nullable array?
                    kind = elementType.BaseType is { SpecialType: SpecialType.System_Enum }
                        ? AttributeParameterTypeKind.EnumArray
                        : AttributeParameterTypeKind.ValueTypeArray;

                    displayString = ToArrayDisplayString(elementType, nullable: false);
                    elementDisplayString = ToDisplayString(elementType);
                }
                // type array?
                else if(IsType(elementType))
                {
                    // nullable type array?
                    if(isElementTypeNullableAnnotated)
                    {
                        kind = AttributeParameterTypeKind.NullableTypeArray;
                        displayString = ToArrayDisplayString(_nullableTypeSymbolDisplayString, nullable: false);
                        elementDisplayString = _nullableTypeSymbolDisplayString;
                    }
                    // type array
                    else
                    {
                        kind = AttributeParameterTypeKind.TypeArray;
                        displayString = ToArrayDisplayString(_typeSymbolDisplayString, nullable: false);
                        elementDisplayString = _nullableTypeSymbolDisplayString;
                    }
                }
                // reference type array
                else
                {
                    // nullable reference type array?
                    if(isElementTypeNullableAnnotated)
                    {
                        kind = AttributeParameterTypeKind.NullableReferenceTypeArray;
                        displayString = ToArrayDisplayString(elementType, nullable: false);
                        elementDisplayString = ToDisplayString(elementType);
                    }
                    // reference type array?
                    else
                    {
                        kind = AttributeParameterTypeKind.ReferenceTypeArray;
                        displayString = ToArrayDisplayString(elementType, nullable: false);
                        elementDisplayString = ToDisplayString(elementType);
                    }
                }
            }
        }
        // reference type
        else
        {
            // nullable reference type
            if(isTypeNullableAnnotated)
            {
                kind = AttributeParameterTypeKind.NullableReferenceType;
                displayString = elementDisplayString = ToDisplayString(type);
            }
            // reference type
            else
            {
                kind = AttributeParameterTypeKind.ReferenceType;
                displayString = elementDisplayString = ToDisplayString(type);
            }
        }

        var kindString = kind.ToStringFast();
        var nullableDisplayString =
            kind.HasAnyFlagFast(AttributeParameterTypeKind.NullableArray)
            || !kind.HasAnyFlagFast(AttributeParameterTypeKind.Array)
            && kind.HasAnyFlagFast(AttributeParameterTypeKind.Nullable)
            ? displayString
            : $"{displayString}?";

        var result = new AttributeParameterTypeModel(
            kind,
            KindString: kindString,
            DisplayString: displayString,
            ElementDisplayString: elementDisplayString,
            NullableDisplayString: nullableDisplayString);

        return result;
    }
    private static String ToDisplayString(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    private static String ToArrayDisplayString(String elementType, Boolean nullable) => $"global::RhoMicro.CodeAnalysis.Library.Models.Collections.EquatableList<{elementType}>{( nullable ? "?" : "" )}";
    private static String ToArrayDisplayString(ITypeSymbol elementType, Boolean nullable) => ToArrayDisplayString(elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), nullable);
    private static Boolean IsType(ITypeSymbol type) => type is { Name: "Type", ContainingNamespace: { Name: "System", ContainingNamespace: { IsGlobalNamespace: true } } };

    public override Int32 GetHashCode() => DisplayString.GetHashCode();
    public Boolean Equals(AttributeParameterTypeModel other) => DisplayString.Equals(other.DisplayString);
}

