// SPDX-License-Identifier: MPL-2.0

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

public partial class IntDouble : IUnion, IEquatable<IntDouble>
{
#region VariantGroupKinds

    [Flags]
    public enum VariantGroupKinds : int
    {
        None = 0,
        Number = 1 << 0,
        Integer = 1 << 1,
        ValueType = 1 << 3
    }

#endregion

#region VariantKind

    public enum VariantKind : byte
    {
        Int32,
        Double
    }

#endregion

#region Factory

    readonly struct Factory : IUnionFactory<IntDouble>
    {
        public Boolean TryCreate<TVariant>(TVariant value, [NotNullWhen(true)] out IntDouble? union)
        {
            switch (value)
            {
                // switch against all variants
                case Int32 v: return IntDouble.TryCreate(v, out union);
                case Double v: return IntDouble.TryCreate(v, out union);
                // By supporting the union itself as a variant, we avoid SOE when matching IUnion,
                // which would otherwise recurse into this method.
                case IntDouble v:
                    union = new IntDouble(v);
                    return true;
                // TODO: detect/register related unions, match here (before IUnion box)

                // attempt to convert to this union through double dispatch, this will
                // effectively convert between related unions, albeit while incurring boxing
                case IUnion v: return v.TryMapTo(this, out union);
                // switch against nullable variants here
                /*
                 * case null:
                 * in this placeholder code, variants for string? and SomeVirtualClass? are assumed
                 * first, we check exact matches:
                 *
                 * if(typeof(TVariant) == typeof(string))
                 * {
                 *      return Union.TryCreate((string?)null, out union);
                 * }
                 * if(typeof(TVariant) == typeof(SomeVirtualClass))
                 * {
                 *      return Union.TryCreate((SomeVirtualClass?)null, out union);
                 * }
                 *
                 * next, we check polymorphic relationships on non-sealed variants:
                 *
                 * if(typeof(SomeVirtualClass).IsAssignableFrom(typeof(TVariant)))
                 * {
                 *      return Union.TryCreate((SomeVirtualClass?)null, out union);
                 * }
                 */
                default:
                    union = default;
                    return false;
            }
        }

        public IntDouble Create<TVariant>(TVariant value)
        {
            // factory is responsible for exact variant match conversion
            switch (value)
            {
                // switch against all variants
                case Int32 v: return new IntDouble(v);
                case Double v: return new IntDouble(v);
                // By supporting the union itself as a variant, we avoid SOE when matching IUnion,
                // which would otherwise recurse into this method.
                case IntDouble v: return new IntDouble(v);
                // TODO: detect/register related unions, match here (before IUnion box)

                // attempt to convert to this union through double dispatch, this will
                // effectively convert between related unions, albeit while incurring boxing
                case IUnion v: return v.MapTo<IntDouble, Factory>(this);
                case null:
                // switch against nullable variants here
                /*
                 * in this placeholder code, variants for string? and SomeVirtualClass? are assumed
                 * first, we check exact matches:
                 *
                 * if(typeof(TVariant) == typeof(string))
                 * {
                 *      return Union.Create((string?)null);
                 * }
                 * if(typeof(TVariant) == typeof(SomeVirtualClass))
                 * {
                 *      return Union.Create((SomeVirtualClass?)null);
                 * }
                 *
                 * next, we check polymorphic relationships on non-sealed variants:
                 *
                 * if(typeof(SomeVirtualClass).IsAssignableFrom(typeof(TVariant)))
                 * {
                 *      return Union.Create((SomeVirtualClass?)null);
                 * }
                 */
                default:
                    throw new ArgumentOutOfRangeException(
                            nameof(value),
                            value,
                            $"Unable to create an instance of '{typeof(IntDouble)}' from a value of type '{value?.GetType() ?? typeof(TVariant)}'");
            }
        }
    }

#endregion

#region UnmanagedVariantsContainer

    [StructLayout(LayoutKind.Explicit)]
    private readonly struct UnmanagedVariantsContainer
    {
        public UnmanagedVariantsContainer(Int32 value)
        {
            Int32 = value;
        }

        public UnmanagedVariantsContainer(Double value)
        {
            Double = value;
        }

        [FieldOffset(0)] public readonly Int32 Int32;
        [FieldOffset(0)] public readonly Double Double;
    }

#endregion

#region Constructors

    public IntDouble(IntDouble prototype)
    {
    }

    public IntDouble(Int32 value) : this(value, validate: true)
    {
    }

    public IntDouble(Double value) : this(value, validate: true)
    {
    }

    private IntDouble(Int32 value, Boolean validate)
    {
        if (validate)
        {
            var isValid = true;
            Validate(value, throwIfInvalid: true, ref isValid);
        }

        // assign reference container with null
        // omit if no reference variants
        // __referenceVariantsContainer = null!;
        // assign unmanaged container if unmanaged variant
        __unmanagedVariantsContainer = new UnmanagedVariantsContainer(value);
        // enumerate unused managed variant field assignments with default 

        Variant = VariantKind.Int32;
    }

    private IntDouble(Double value, Boolean validate)
    {
        if (validate)
        {
            var isValid = true;
            Validate(value, throwIfInvalid: true, ref isValid);
        }

        // assign reference container with null
        // omit if no reference variants
        // __referenceVariantsContainer = null!;
        // assign unmanaged container if unmanaged variant
        __unmanagedVariantsContainer = new UnmanagedVariantsContainer(value);
        // enumerate unused managed variant field assignments with default

        Variant = VariantKind.Double;
    }

#endregion

#region Fields

    private readonly UnmanagedVariantsContainer __unmanagedVariantsContainer;
    // private readonly Object __referenceVariantsContainer;
    // enumerate fields for managed structs, make boxing optional

#endregion

#region Properties

    public VariantKind Variant { get; }

    public Object Value
    {
        get
        {
            Object result = Variant switch
            {
                    VariantKind.Int32 => __unmanagedVariantsContainer.Int32,
                    VariantKind.Double => __unmanagedVariantsContainer.Double,
                    _ => throw new InvalidOperationException(
                            $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
            };

            return result;
        }
    }

    public Boolean IsInt32 => Variant is VariantKind.Int32;
    public Int32 AsInt32 => __unmanagedVariantsContainer.Int32;

    public Int32 ToInt32 => IsInt32
            ? AsInt32
            : throw new InvalidOperationException(
                    $"Unable to convert union to 'Int32', as it is currently representing the '{Variant}' variant.");

    public Boolean IsDouble => Variant is VariantKind.Double;
    public Double AsDouble => __unmanagedVariantsContainer.Double;

    public Double ToDouble => IsDouble
            ? AsDouble
            : throw new InvalidOperationException(
                    $"Unable to convert union to 'Double', as it is currently representing the '{Variant}' variant.");

#endregion

#region Is/As Methods

    // warn that value is guaranteed to be null for class TVariant and false return
    // attach [NNW(true)] and TVariant? when not representing any nullable reference variants
    public Boolean TryAs<TVariant>(out TVariant value)
    {
        // TODO: fix variant comparisons not actually checking the currently represented variant
        var variant = typeof(TVariant);
        value = default!;

        if (variant == typeof(Int32))
        {
            if (IsInt32)
            {
                var fromValue = AsInt32;
                value = Unsafe.As<Int32, TVariant>(ref fromValue);
                return true;
            }
        }

        if (variant == typeof(Double))
        {
            var fromValue = AsDouble;
            value = Unsafe.As<Double, TVariant>(ref fromValue);
            return IsDouble;
        }

        if (variant == typeof(IntDouble))
        {
            return true;
        }

        if (variant == typeof(Object))
        {
            return true;
        }

        // include this check if we have value type variants
        if (variant == typeof(Enum))
        {
            // disjunct all value type variants
            return Variant is VariantKind.Int32 or VariantKind.Double;
        }

        return false;
    }

    public Boolean Is<TVariant>()
    {
        var variant = typeof(TVariant);

        if (variant == Variant.Type)
        {
            return true;
        }

        if (variant == typeof(IntDouble))
        {
            return true;
        }

        if (variant == typeof(Object))
        {
            return true;
        }

        // include this check if we have value type variants, replicate for enum types
        if (variant == typeof(ValueType))
        {
            // disjunct all value type variants
            return Variant is VariantKind.Int32 or VariantKind.Double;
        }

        return false;
    }

#endregion

#region Validation

    static partial void Validate(Int32 value, Boolean throwIfInvalid, ref Boolean isValid);
    static partial void Validate(Double value, Boolean throwIfInvalid, ref Boolean isValid);

#endregion

#region Factories

    public static IntDouble Create(IntDouble value) => new(value);

    public static Boolean TryCreate(IntDouble value, [NotNullWhen(true)] out IntDouble? union)
    {
        union = Create(value);

        return true;
    }

    public static IntDouble Create(Int32 value) => new IntDouble(value, validate: true);

    public static Boolean TryCreate(Int32 value, [NotNullWhen(true)] out IntDouble? union)
    {
        var isValid = true;
        Validate(value, throwIfInvalid: false, ref isValid);
        union = isValid
                ? new IntDouble(value, validate: false)
                : default;

        return isValid;
    }

    public static IntDouble Create(Double value) => new IntDouble(value, validate: true);

    public static Boolean TryCreate(Double value, [NotNullWhen(true)] out IntDouble? union)
    {
        var isValid = true;
        Validate(value, throwIfInvalid: false, ref isValid);
        union = isValid
                ? new IntDouble(value, validate: false)
                : default;

        return isValid;
    }

    public static IntDouble Create<T>(T value)
        => new Factory().Create(value);

    public static Boolean TryCreate<T>(T value, [NotNullWhen(true)] out IntDouble? union)
        => new Factory().TryCreate(value, out union);

#endregion

#region Mapping

    // this method is intended for inter-union conversion only
    public TUnion MapTo<TUnion, TFactory>(
            TFactory factory)
            where TFactory : IUnionFactory<TUnion>
    {
        var result = Variant switch
        {
                VariantKind.Int32 => factory.Create(__unmanagedVariantsContainer.Int32),
                VariantKind.Double => factory.Create(__unmanagedVariantsContainer.Double),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

    // this method is intended for inter-union conversion only
    public Boolean TryMapTo<TUnion, TFactory>(
            TFactory factory,
            [NotNullWhen(true)] out TUnion? union)
            where TFactory : IUnionFactory<TUnion>
    {
        var result = Variant switch
        {
                VariantKind.Int32 => factory.TryCreate(__unmanagedVariantsContainer.Int32, out union),
                VariantKind.Double => factory.TryCreate(__unmanagedVariantsContainer.Double, out union),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

#endregion

#region Equality

    public override Boolean Equals(Object? obj) => obj is IntDouble union && Equals(union);

    public Boolean Equals(IntDouble other)
    {
        // only emit for reference unions
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (Variant != other.Variant)
        {
            return false;
        }

        var result = Variant switch
        {
                VariantKind.Int32 =>
                        EqualityComparer<Int32>.Default.Equals(
                                __unmanagedVariantsContainer.Int32,
                                other.__unmanagedVariantsContainer.Int32),
                VariantKind.Double =>
                        EqualityComparer<Double>.Default.Equals(
                                __unmanagedVariantsContainer.Double,
                                other.__unmanagedVariantsContainer.Double),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

    public override Int32 GetHashCode()
    {
        var result = Variant switch
        {
                VariantKind.Int32 => HashCode.Combine(Variant, __unmanagedVariantsContainer.Int32),
                VariantKind.Double => HashCode.Combine(__unmanagedVariantsContainer.Double),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

#endregion

#region Conversion

    public static implicit operator IntDouble(Int32 value) => Create(value);
    public static explicit operator Int32(IntDouble union) => union.ToInt32;

    public static implicit operator IntDouble(Double value) => Create(value);
    public static explicit operator Double(IntDouble union) => union.ToDouble;

#endregion
}

public static partial class IntDoubleVariantGroupKindsOperations
{
    extension(IntDouble.VariantGroupKinds @this)
    {
        public Boolean HasFlag(IntDouble.VariantGroupKinds flag) => (@this & flag) == flag;

        public static ImmutableArray<IntDouble.VariantGroupKinds> GetAllValues() =>
        [
                IntDouble.VariantGroupKinds.Number,
                IntDouble.VariantGroupKinds.Integer,
                IntDouble.VariantGroupKinds.ValueType
        ];

        public Int32 IndividualGroupCount => PopCount((UInt32)@this);

        private void GetIndividualGroups(Span<IntDouble.VariantGroupKinds> buffer)
        {
            var count = @this.IndividualGroupCount;

            if (count is 0)
            {
                return;
            }

            if (buffer.Length < count)
            {
                throw new ArgumentOutOfRangeException(
                        nameof(buffer),
                        $"{nameof(buffer)} did not have the required length of IndividualGroupCount. The required length was {count}, but the span provided had a length of {buffer.Length}.");
            }

            if (count is 1)
            {
                buffer[0] = @this;
                return;
            }

            var groupIndex = 0;
            for (var i = LeadingZeroCount((UInt32)@this) + 1; i < 33 && groupIndex < count; i++)
            {
                var flagPosition = 32 - i;
                var flag = 1 << flagPosition;
                if (((Int32)@this & flag) == flag)
                {
                    buffer[groupIndex++] = (IntDouble.VariantGroupKinds)flag;
                }
            }
        }

        public ImmutableArray<IntDouble.VariantGroupKinds> GetIndividualGroups()
        {
            var groups = new IntDouble.VariantGroupKinds[@this.IndividualGroupCount];
            @this.GetIndividualGroups(groups);
            var result = ImmutableCollectionsMarshal.AsImmutableArray(groups);

            return result;
        }

        public String Name
        {
            get
            {
                var count = @this.IndividualGroupCount;

                if (count is 0 or 1)
                {
                    return @this.DegenerateName;
                }

                Span<IntDouble.VariantGroupKinds>
                        groups = stackalloc IntDouble.VariantGroupKinds[count]; // Count never exceeds 32
                @this.GetIndividualGroups(groups);
                var builder = new StringBuilder();
                for (var i = 0; i < count; i++)
                {
                    var group = groups[i];
                    var name = group.DegenerateName;

                    if (i is not 0)
                    {
                        builder.Append(" | ");
                    }

                    builder.Append(name);
                }

                var result = builder.ToString();

                return result;
            }
        }

        private String DegenerateName
        {
            get
            {
                return @this switch
                {
                        IntDouble.VariantGroupKinds.None => nameof(IntDouble.VariantGroupKinds.None),
                        IntDouble.VariantGroupKinds.Number => nameof(IntDouble.VariantGroupKinds.Number),
                        IntDouble.VariantGroupKinds.Integer => nameof(IntDouble.VariantGroupKinds.Integer),
                        IntDouble.VariantGroupKinds.ValueType => nameof(IntDouble.VariantGroupKinds
                                                                                             .ValueType),
                        _ => throw new InvalidOperationException(
                                $"The VariantGroups instance was not initialized correctly and is holding an invalid value: {@this}")
                };
            }
        }
    }

    private static ReadOnlySpan<Byte> Log2DeBruijn =>
    [
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
    ];

    private static Int32 LeadingZeroCount(UInt32 value)
    {
        if (value == 0)
        {
            return 32;
        }

        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;

        var result = 31 ^ Unsafe.AddByteOffset(
                ref MemoryMarshal.GetReference(Log2DeBruijn),
                (IntPtr)(Int32)((value * 0x07C4ACDDu) >> 27));

        return result;
    }

    private static Int32 PopCount(UInt32 value)
    {
        const UInt32 c1 = 0x_55555555u;
        const UInt32 c2 = 0x_33333333u;
        const UInt32 c3 = 0x_0F0F0F0Fu;
        const UInt32 c4 = 0x_01010101u;

        value -= (value >> 1) & c1;
        value = (value & c2) + ((value >> 2) & c2);
        value = (((value + (value >> 4)) & c3) * c4) >> 24;

        return (Int32)value;
    }
}

public static partial class IntDoubleVariantKindsOperations
{
    extension(IntDouble.VariantKind @this)
    {
        public static ImmutableArray<IntDouble.VariantKind> GetAllValues() =>
        [
                IntDouble.VariantKind.Int32,
                IntDouble.VariantKind.Double,
        ];

        public String Name
        {
            get
            {
                var name = @this switch
                {
                        IntDouble.VariantKind.Int32 => nameof(Int32),
                        IntDouble.VariantKind.Double => nameof(Double),
                        _ => throw new InvalidOperationException(
                                "Unable to determine name, as the variant was not initialized correctly.")
                };

                return name;
            }
        }

        public Type Type
        {
            get
            {
                var type = @this switch
                {
                        IntDouble.VariantKind.Int32 => typeof(Int32),
                        IntDouble.VariantKind.Double => typeof(Double),
                        _ => throw new InvalidOperationException(
                                "Unable to determine type, as the variant was not initialized correctly.")
                };

                return type;
            }
        }

        public IntDouble.VariantGroupKinds Groups
        {
            get
            {
                var groups = @this switch
                {
                        IntDouble.VariantKind.Int32 => IntDouble.VariantGroupKinds.Number |
                                                             IntDouble.VariantGroupKinds.Integer |
                                                             IntDouble.VariantGroupKinds.ValueType,
                        IntDouble.VariantKind.Double => IntDouble.VariantGroupKinds.Number |
                                                              IntDouble.VariantGroupKinds.ValueType,
                        _ => throw new InvalidOperationException(
                                "Unable to determine groups, as the variant was not initialized correctly.")
                };

                return groups;
            }
        }
    }
}
