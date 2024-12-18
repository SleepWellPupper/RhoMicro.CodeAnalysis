namespace RhoMicro.CodeAnalysis.Library.Models;

using System.Runtime.CompilerServices;

[Flags]
#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal enum AttributeParameterTypeKind
{
    ReferenceType = 1,
    Type = 2,
    ValueType = 4,

    Nullable = 8,
    Array = 16,
    NullableArray = 32,

    NullableType = Nullable | Type,
    NullableReferenceType = Nullable | ReferenceType,

    NullableTypeArray = Nullable | Type | Array,
    NullableReferenceTypeArray = Nullable | ReferenceType | Array,

    TypeArray = Type | Array,
    ValueTypeArray = ValueType | Array,
    ReferenceTypeArray = ReferenceType | Array,

    TypeNullableArray = Type | NullableArray,
    ValueTypeNullableArray = ValueType | NullableArray,
    ReferenceTypeNullableArray = ReferenceType | NullableArray,

    NullableTypeNullableArray = Nullable | Type | NullableArray,
    NullableReferenceTypeNullableArray = Nullable | ReferenceType | NullableArray,
}

internal static class AttributeArgumentTypeKindExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasFlagsFast(this AttributeParameterTypeKind value, params ReadOnlySpan<AttributeParameterTypeKind> flags)
    {
        foreach(var flag in flags)
        {
            if(( value & flag ) == flag)
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String ToStringFast(this AttributeParameterTypeKind value) => value switch
    {
        AttributeParameterTypeKind.ReferenceType => nameof(AttributeParameterTypeKind.ReferenceType),
        AttributeParameterTypeKind.Type => nameof(AttributeParameterTypeKind.Type),
        AttributeParameterTypeKind.ValueType => nameof(AttributeParameterTypeKind.ValueType),
        AttributeParameterTypeKind.NullableType => nameof(AttributeParameterTypeKind.NullableType),
        AttributeParameterTypeKind.NullableReferenceType => nameof(AttributeParameterTypeKind.NullableReferenceType),
        AttributeParameterTypeKind.NullableTypeArray => nameof(AttributeParameterTypeKind.NullableTypeArray),
        AttributeParameterTypeKind.NullableReferenceTypeArray => nameof(AttributeParameterTypeKind.NullableReferenceTypeArray),
        AttributeParameterTypeKind.TypeArray => nameof(AttributeParameterTypeKind.TypeArray),
        AttributeParameterTypeKind.ValueTypeArray => nameof(AttributeParameterTypeKind.ValueTypeArray),
        AttributeParameterTypeKind.ReferenceTypeArray => nameof(AttributeParameterTypeKind.ReferenceTypeArray),
        AttributeParameterTypeKind.TypeNullableArray => nameof(AttributeParameterTypeKind.TypeNullableArray),
        AttributeParameterTypeKind.ValueTypeNullableArray => nameof(AttributeParameterTypeKind.ValueTypeNullableArray),
        AttributeParameterTypeKind.ReferenceTypeNullableArray => nameof(AttributeParameterTypeKind.ReferenceTypeNullableArray),
        AttributeParameterTypeKind.NullableTypeNullableArray => nameof(AttributeParameterTypeKind.NullableTypeNullableArray),
        AttributeParameterTypeKind.NullableReferenceTypeNullableArray => nameof(AttributeParameterTypeKind.NullableReferenceTypeNullableArray),
        _ => String.Empty
    };
}