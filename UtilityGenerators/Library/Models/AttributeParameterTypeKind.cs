// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Library.Models;
[Flags]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal enum AttributeParameterTypeKind
{
    ReferenceType = 1,
    Type = 2,
    ValueType = 4,
    Enum = 8,

    Nullable = 16,
    Array = 32,
    NullableArray = 64,

    NullableType = Nullable | Type,
    NullableReferenceType = Nullable | ReferenceType,

    NullableTypeArray = Nullable | Type | Array,
    NullableReferenceTypeArray = Nullable | ReferenceType | Array,

    TypeArray = Type | Array,
    ValueTypeArray = ValueType | Array,
    EnumArray = Enum | Array,
    ReferenceTypeArray = ReferenceType | Array,

    TypeNullableArray = Type | NullableArray,
    ValueTypeNullableArray = ValueType | NullableArray,
    EnumNullableArray = Enum | NullableArray,
    ReferenceTypeNullableArray = ReferenceType | NullableArray,

    NullableTypeNullableArray = Nullable | Type | NullableArray,
    NullableReferenceTypeNullableArray = Nullable | ReferenceType | NullableArray,
}

internal static class AttributeArgumentTypeKindExtensions
{
    public static Boolean HasAnyFlagFast(this AttributeParameterTypeKind value, params ReadOnlySpan<AttributeParameterTypeKind> flags)
    {
        foreach(var flag in flags)
        {
            if(( value & flag ) == flag)
            {
                return true;
            }
        }

        return false;
    }

    public static String ToStringFast(this AttributeParameterTypeKind value) => value switch
    {
        AttributeParameterTypeKind.ReferenceType => nameof(AttributeParameterTypeKind.ReferenceType),
        AttributeParameterTypeKind.Type => nameof(AttributeParameterTypeKind.Type),
        AttributeParameterTypeKind.ValueType => nameof(AttributeParameterTypeKind.ValueType),
        AttributeParameterTypeKind.Enum => nameof(AttributeParameterTypeKind.Enum),
        AttributeParameterTypeKind.NullableType => nameof(AttributeParameterTypeKind.NullableType),
        AttributeParameterTypeKind.NullableReferenceType => nameof(AttributeParameterTypeKind.NullableReferenceType),
        AttributeParameterTypeKind.NullableTypeArray => nameof(AttributeParameterTypeKind.NullableTypeArray),
        AttributeParameterTypeKind.NullableReferenceTypeArray => nameof(AttributeParameterTypeKind.NullableReferenceTypeArray),
        AttributeParameterTypeKind.TypeArray => nameof(AttributeParameterTypeKind.TypeArray),
        AttributeParameterTypeKind.ValueTypeArray => nameof(AttributeParameterTypeKind.ValueTypeArray),
        AttributeParameterTypeKind.EnumArray => nameof(AttributeParameterTypeKind.EnumArray),
        AttributeParameterTypeKind.ReferenceTypeArray => nameof(AttributeParameterTypeKind.ReferenceTypeArray),
        AttributeParameterTypeKind.TypeNullableArray => nameof(AttributeParameterTypeKind.TypeNullableArray),
        AttributeParameterTypeKind.ValueTypeNullableArray => nameof(AttributeParameterTypeKind.ValueTypeNullableArray),
        AttributeParameterTypeKind.EnumNullableArray => nameof(AttributeParameterTypeKind.EnumNullableArray),
        AttributeParameterTypeKind.ReferenceTypeNullableArray => nameof(AttributeParameterTypeKind.ReferenceTypeNullableArray),
        AttributeParameterTypeKind.NullableTypeNullableArray => nameof(AttributeParameterTypeKind.NullableTypeNullableArray),
        AttributeParameterTypeKind.NullableReferenceTypeNullableArray => nameof(AttributeParameterTypeKind.NullableReferenceTypeNullableArray),
        _ => String.Empty
    };
}
