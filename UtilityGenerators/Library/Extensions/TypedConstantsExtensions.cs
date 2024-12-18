namespace RhoMicro.CodeAnalysis.Library.Extensions;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis;

#if RHOMICRO_CODEANALYSIS_UTILITYGENERATORS
[IncludeFile]
#endif
internal static class TypedConstantsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetReferenceTypeValue<T>(this TypedConstant typedConstant, [NotNullWhen(true)] out T? value)
        where T : class
    {
        if(typedConstant is { Kind: TypedConstantKind.Primitive, Value: T v })
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableReferenceTypeValue<T>(this TypedConstant typedConstant, out T? value)
        where T : class
    {
        if(typedConstant is { Kind: TypedConstantKind.Primitive, Value: T or null } c)
        {
            value = c.Value as T;
            return true;
        }

        value = null;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetValueTypeValue<T>(this TypedConstant typedConstant, [NotNullWhen(true)] out T? value)
        where T : struct
    {
        if(typedConstant is { Kind: TypedConstantKind.Primitive, Value: T v })
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetTypeValue(this TypedConstant typedConstant, [NotNullWhen(true)] out ITypeSymbol? value)
    {
        if(typedConstant is { Kind: TypedConstantKind.Type, Value: ITypeSymbol v })
        {
            value = v;
            return true;
        }

        value = null;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableTypeValue(this TypedConstant typedConstant, out ITypeSymbol? value)
    {
        if(typedConstant is { Kind: TypedConstantKind.Type, Value: ITypeSymbol or null } c)
        {
            value = c.Value as ITypeSymbol;
            return true;
        }

        value = null;
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetReferenceTypeArrayValue<T>(this TypedConstant typedConstant, [NotNullWhen(true)] out ImmutableArray<T>? value)
        where T : class
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array
            })
        {
            value = null;
            return false;
        }

        return TryGetReferenceTypeArrayValue(array, out value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetReferenceTypeNullableArrayValue<T>(this TypedConstant typedConstant, out ImmutableArray<T>? value)
        where T : class
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array,
                IsNull: { } isNull
            })
        {
            value = null;
            return false;
        }

        if(isNull)
        {
            value = null;
            return true;
        }

        return TryGetReferenceTypeArrayValue(array, out value);
    }
    private static Boolean TryGetReferenceTypeArrayValue<T>(ImmutableArray<TypedConstant> array, [NotNullWhen(true)] out ImmutableArray<T>? value)
        where T : class
    {
        var builder = ImmutableArray.CreateBuilder<T>(array.Length);

        for(var i = 0; i < array.Length; i++)
        {
            if(array[i].TryGetReferenceTypeValue<T>(out var item))
            {
                builder[i] = item;
            } else
            {
                value = null;
                return false;
            }
        }

        value = builder.MoveToImmutable();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableReferenceTypeArrayValue<T>(this TypedConstant typedConstant, [NotNullWhen(true)] out ImmutableArray<T?>? value)
        where T : class
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array
            })
        {
            value = null;
            return false;
        }

        return TryGetNullableReferenceTypeArrayValue(array, out value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableReferenceTypeNullableArrayValue<T>(this TypedConstant typedConstant, out ImmutableArray<T?>? value)
        where T : class
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array,
                IsNull: { } isNull
            })
        {
            value = null;
            return false;
        }

        if(isNull)
        {
            value = null;
            return true;
        }

        return TryGetNullableReferenceTypeArrayValue(array, out value);
    }
    private static Boolean TryGetNullableReferenceTypeArrayValue<T>(ImmutableArray<TypedConstant> array, [NotNullWhen(true)] out ImmutableArray<T?>? value)
        where T : class
    {
        var builder = ImmutableArray.CreateBuilder<T?>(array.Length);

        for(var i = 0; i < array.Length; i++)
        {
            if(array[i].TryGetNullableReferenceTypeValue<T>(out var item))
            {
                builder[i] = item;
            } else
            {
                value = null;
                return false;
            }
        }

        value = builder.MoveToImmutable();
        return true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetValueTypeArrayValue<T>(this TypedConstant typedConstant, [NotNullWhen(true)] out ImmutableArray<T>? value)
        where T : struct
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array
            })
        {
            value = null;
            return false;
        }

        return TryGetValueTypeArrayValue(array, out value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetValueTypeNullableArrayValue<T>(this TypedConstant typedConstant, out ImmutableArray<T>? value)
        where T : struct
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array,
                IsNull: { } isNull
            })
        {
            value = null;
            return false;
        }

        if(isNull)
        {
            value = null;
            return true;
        }

        return TryGetValueTypeArrayValue(array, out value);
    }
    private static Boolean TryGetValueTypeArrayValue<T>(ImmutableArray<TypedConstant> array, [NotNullWhen(true)] out ImmutableArray<T>? value)
        where T : struct
    {
        var builder = ImmutableArray.CreateBuilder<T>(array.Length);

        for(var i = 0; i < array.Length; i++)
        {
            if(array[i].TryGetValueTypeValue<T>(out var item))
            {
                builder[i] = item!.Value;
            } else
            {
                value = null;
                return false;
            }
        }

        value = builder.MoveToImmutable();
        return true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetTypeArrayValue(this TypedConstant typedConstant, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value)
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array
            })
        {
            value = null;
            return false;
        }

        return TryGetTypeArrayValue(array, out value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetTypeNullableArrayValue(this TypedConstant typedConstant, out ImmutableArray<ITypeSymbol>? value)
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array,
                IsNull: { } isNull
            })
        {
            value = null;
            return false;
        }

        if(isNull)
        {
            value = null;
            return true;
        }

        return TryGetTypeArrayValue(array, out value);
    }
    private static Boolean TryGetTypeArrayValue(ImmutableArray<TypedConstant> array, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol>? value)
    {
        var builder = ImmutableArray.CreateBuilder<ITypeSymbol>(array.Length);

        for(var i = 0; i < array.Length; i++)
        {
            if(array[i].TryGetTypeValue(out var item))
            {
                builder[i] = item;
            } else
            {
                value = null;
                return false;
            }
        }

        value = builder.MoveToImmutable();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableTypeArrayValue(this TypedConstant typedConstant, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol?>? value)
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array
            })
        {
            value = null;
            return false;
        }

        return TryGetNullableTypeArrayValue(array, out value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean TryGetNullableTypeNullableArrayValue(this TypedConstant typedConstant, out ImmutableArray<ITypeSymbol?>? value)
    {
        if(typedConstant is not
            {
                Kind: TypedConstantKind.Array,
                Values: { } array,
                IsNull: { } isNull
            })
        {
            value = null;
            return false;
        }

        if(isNull)
        {
            value = null;
            return true;
        }

        return TryGetNullableTypeArrayValue(array, out value);
    }
    private static Boolean TryGetNullableTypeArrayValue(ImmutableArray<TypedConstant> array, [NotNullWhen(true)] out ImmutableArray<ITypeSymbol?>? value)
    {
        var builder = ImmutableArray.CreateBuilder<ITypeSymbol?>(array.Length);

        for(var i = 0; i < array.Length; i++)
        {
            if(array[i].TryGetNullableTypeValue(out var item))
            {
                builder[i] = item;
            } else
            {
                value = null;
                return false;
            }
        }

        value = builder.MoveToImmutable();
        return true;
    }
}
