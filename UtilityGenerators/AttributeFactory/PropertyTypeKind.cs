namespace RhoMicro.CodeAnalysis;

using System.Runtime.CompilerServices;

[Flags]
internal enum PropertyTypeKind
{
    ReferenceType = 1,
    Type = 2,
    ValueType = 4,

    Nullable = 8,
    Array = 16,
    NullableArray = 32,

    NullableType = Nullable | Type,
    NullableReferenceType = Nullable | ReferenceType,

    TypeArray = Type | Array,
    ValueTypeArray = ValueType | Array,
    ReferenceTypeArray = ReferenceType | Array,

    TypeNullableArray = Type | NullableArray,
    ValueTypeNullableArray = ValueType | NullableArray,
    ReferenceTypeNullableArray = ReferenceType | NullableArray,

    NullableTypeNullableArray = Nullable | Type | NullableArray,
    NullableReferenceTypeNullableArray = Nullable | ReferenceType | NullableArray,
}

internal static class PropertyTypeKindExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean HasFlagFast(this PropertyTypeKind value, PropertyTypeKind flag) => ( value & flag ) == flag;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String ToStringFast(this PropertyTypeKind value) => value switch
    {
        PropertyTypeKind.ReferenceType => nameof(PropertyTypeKind.ReferenceType),
        PropertyTypeKind.Type => nameof(PropertyTypeKind.Type),
        PropertyTypeKind.ValueType => nameof(PropertyTypeKind.ValueType),
        PropertyTypeKind.NullableType => nameof(PropertyTypeKind.NullableType),
        PropertyTypeKind.NullableReferenceType => nameof(PropertyTypeKind.NullableReferenceType),
        PropertyTypeKind.TypeArray => nameof(PropertyTypeKind.TypeArray),
        PropertyTypeKind.ValueTypeArray => nameof(PropertyTypeKind.ValueTypeArray),
        PropertyTypeKind.ReferenceTypeArray => nameof(PropertyTypeKind.ReferenceTypeArray),
        PropertyTypeKind.TypeNullableArray => nameof(PropertyTypeKind.TypeNullableArray),
        PropertyTypeKind.ValueTypeNullableArray => nameof(PropertyTypeKind.ValueTypeNullableArray),
        PropertyTypeKind.ReferenceTypeNullableArray => nameof(PropertyTypeKind.ReferenceTypeNullableArray),
        PropertyTypeKind.NullableTypeNullableArray => nameof(PropertyTypeKind.NullableTypeNullableArray),
        PropertyTypeKind.NullableReferenceTypeNullableArray => nameof(PropertyTypeKind.NullableReferenceTypeNullableArray),
        _ => String.Empty
    };
}