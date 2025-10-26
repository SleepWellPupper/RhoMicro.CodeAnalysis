// SPDX-License-Identifier: MPL-2.0

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

public partial class IntDoubleString : IUnion, IEquatable<IntDoubleString>
{
#region VariantGroupKinds

    [Flags]
    public enum VariantGroupKinds : byte
    {
        None = 0,
        Number = 1 << 0,
        Integer = 1 << 1,
        ReferenceType = 1 << 2,
        ValueType = 1 << 3
    }

#endregion

#region VariantKind

    public enum VariantKind : byte
    {
        Int32,
        Double,
        String
    }

#endregion

#region Factory

    readonly struct Factory : IUnionFactory<IntDoubleString>
    {
        public Boolean TryCreate<TVariant>(TVariant value, [NotNullWhen(true)] out IntDoubleString? union)
        {
            switch (value)
            {
                // switch against all variants
                case Int32 v: return IntDoubleString.TryCreate(v, out union);
                case Double v: return IntDoubleString.TryCreate(v, out union);
                case String v: return IntDoubleString.TryCreate(v, out union);
                // By supporting the union itself as a variant, we avoid SOE when matching IUnion,
                // which would otherwise recurse into this method.
                case IntDoubleString v:
                    union = new IntDoubleString(v);
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

        public IntDoubleString Create<TVariant>(TVariant value)
        {
            // factory is responsible for exact variant match conversion
            switch (value)
            {
                // switch against all variants
                case Int32 v: return new IntDoubleString(v);
                case Double v: return new IntDoubleString(v);
                case String v: return new IntDoubleString(v);
                // By supporting the union itself as a variant, we avoid SOE when matching IUnion,
                // which would otherwise recurse into this method.
                case IntDoubleString v: return new IntDoubleString(v);
                // TODO: detect/register related unions, match here (before IUnion box)

                // attempt to convert to this union through double dispatch, this will
                // effectively convert between related unions, albeit while incurring boxing
                case IUnion v: return v.MapTo<IntDoubleString, Factory>(this);
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
                            $"Unable to create an instance of '{typeof(IntDoubleString)}' from a value of type '{value?.GetType() ?? typeof(TVariant)}'");
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

    public IntDoubleString(IntDoubleString prototype)
    {
    }

    public IntDoubleString(Int32 value) : this(value, validate: true)
    {
    }

    public IntDoubleString(Double value) : this(value, validate: true)
    {
    }

    public IntDoubleString(String value) : this(value, validate: true)
    {
    }

    private IntDoubleString(Int32 value, Boolean validate)
    {
        if (validate)
        {
            var isValid = true;
            Validate(value, throwIfInvalid: true, ref isValid);
        }

        // assign reference container with null
        // omit if no reference variants
        __referenceVariantsContainer = null!;
        // assign unmanaged container if unmanaged variant
        __unmanagedVariantsContainer = new UnmanagedVariantsContainer(value);
        // enumerate unused managed variant field assignments with default 

        Variant = VariantKind.Int32;
    }

    private IntDoubleString(Double value, Boolean validate)
    {
        if (validate)
        {
            var isValid = true;
            Validate(value, throwIfInvalid: true, ref isValid);
        }

        // assign reference container with null
        // omit if no reference variants
        __referenceVariantsContainer = null!;
        // assign unmanaged container if unmanaged variant
        __unmanagedVariantsContainer = new UnmanagedVariantsContainer(value);
        // enumerate unused managed variant field assignments with default

        Variant = VariantKind.Double;
    }

    private IntDoubleString(String value, Boolean validate)
    {
        if (validate)
        {
            var isValid = true;
            Validate(value, throwIfInvalid: true, ref isValid);
        }

        // assign reference container with null
        // omit if no reference variants
        __referenceVariantsContainer = value;
        // assign unmanaged container if unmanaged variants exist
        __unmanagedVariantsContainer = default;
        // enumerate unused managed variant field assignments with default

        Variant = VariantKind.String;
    }

#endregion

#region Fields

    private readonly UnmanagedVariantsContainer __unmanagedVariantsContainer;
    private readonly Object __referenceVariantsContainer;
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
                    VariantKind.String => __referenceVariantsContainer,
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

    // MNNW for non-nullable reference variants
    [MemberNotNullWhen(true, nameof(AsString))]
    public Boolean IsString => Variant is VariantKind.String;

    public String? AsString => (String?)__referenceVariantsContainer;

    public String ToString => IsString
            ? AsString
            : throw new InvalidOperationException(
                    $"Unable to convert union to 'String', as it is currently representing the '{Variant}' variant.");

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

        if (variant == typeof(String))
        {
            var fromValue = AsString;
            value = Unsafe.As<String, TVariant>(ref fromValue);
            return IsString;
        }

        if (variant == typeof(IntDoubleString))
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

        // enumerate reference variants that do not directly inherit from object
        if (variant.IsAssignableFrom(typeof(String)))
        {
            return IsString;
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

        if (variant == typeof(IntDoubleString))
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

        // enumerate reference variants that do not directly inherit from object
        if (variant.IsAssignableFrom(typeof(String)))
        {
            return IsString;
        }

        return false;
    }

#endregion

#region Validation

    static partial void Validate(Int32 value, Boolean throwIfInvalid, ref Boolean isValid);
    static partial void Validate(Double value, Boolean throwIfInvalid, ref Boolean isValid);
    static partial void Validate(String value, Boolean throwIfInvalid, ref Boolean isValid);

#endregion

#region Factories

    public static IntDoubleString Create(IntDoubleString value) => new(value);

    public static Boolean TryCreate(IntDoubleString value, [NotNullWhen(true)] out IntDoubleString? union)
    {
        union = Create(value);

        return true;
    }

    public static IntDoubleString Create(Int32 value) => new IntDoubleString(value, validate: true);

    public static Boolean TryCreate(Int32 value, [NotNullWhen(true)] out IntDoubleString? union)
    {
        var isValid = true;
        Validate(value, throwIfInvalid: false, ref isValid);
        union = isValid
                ? new IntDoubleString(value, validate: false)
                : default;

        return isValid;
    }

    public static IntDoubleString Create(Double value) => new IntDoubleString(value, validate: true);

    public static Boolean TryCreate(Double value, [NotNullWhen(true)] out IntDoubleString? union)
    {
        var isValid = true;
        Validate(value, throwIfInvalid: false, ref isValid);
        union = isValid
                ? new IntDoubleString(value, validate: false)
                : default;

        return isValid;
    }

    public static IntDoubleString Create(String value) => new IntDoubleString(value, validate: true);

    public static Boolean TryCreate(String value, [NotNullWhen(true)] out IntDoubleString? union)
    {
        var isValid = true;
        Validate(value, throwIfInvalid: false, ref isValid);
        union = isValid
                ? new IntDoubleString(value, validate: false)
                : default;

        return isValid;
    }

    public static IntDoubleString Create<T>(T value)
        => new Factory().Create(value);

    public static Boolean TryCreate<T>(T value, [NotNullWhen(true)] out IntDoubleString? union)
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
                VariantKind.String => factory.Create((String)__referenceVariantsContainer),
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
                VariantKind.String => factory.TryCreate((String)__referenceVariantsContainer, out union),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

#endregion

#region Equality

    public override Boolean Equals(Object? obj) => obj is IntDoubleString union && Equals(union);

    public Boolean Equals(IntDoubleString other)
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
                VariantKind.String =>
                        EqualityComparer<String>.Default.Equals(
                                (String)__referenceVariantsContainer,
                                (String)other.__referenceVariantsContainer),
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
                VariantKind.String => HashCode.Combine((String)__referenceVariantsContainer),
                _ => throw new InvalidOperationException(
                        $"Unable to determine the variant of this union, as '{nameof(Variant)}' is not representing a valid variant of this union: '{Variant}'. This could be either because the union itself was not initialized correctly, or due to a bug in the 'UnionsGenerator' that generated this union type. Please report an issue to the maintainer.")
        };

        return result;
    }

#endregion

#region Conversion

    public static implicit operator IntDoubleString(Int32 value) => Create(value);
    public static explicit operator Int32(IntDoubleString union) => union.ToInt32;

    public static implicit operator IntDoubleString(Double value) => Create(value);
    public static explicit operator Double(IntDoubleString union) => union.ToDouble;

    public static implicit operator IntDoubleString(String value) => Create(value);
    public static explicit operator String(IntDoubleString union) => union.ToString;

#endregion
}

public static partial class IntDoubleStringVariantGroupKindsOperations
{
    extension(IntDoubleString.VariantGroupKinds @this)
    {
        public Boolean HasFlag(IntDoubleString.VariantGroupKinds flag) => (@this & flag) == flag;

        public static ImmutableArray<IntDoubleString.VariantGroupKinds> GetAllValues() =>
        [
                IntDoubleString.VariantGroupKinds.Number,
                IntDoubleString.VariantGroupKinds.Integer,
                IntDoubleString.VariantGroupKinds.ReferenceType,
                IntDoubleString.VariantGroupKinds.ValueType
        ];

        public Int32 IndividualGroupCount => PopCount((UInt32)@this);

        private void GetIndividualGroups(Span<IntDoubleString.VariantGroupKinds> buffer)
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
                    buffer[groupIndex++] = (IntDoubleString.VariantGroupKinds)flag;
                }
            }
        }

        public ImmutableArray<IntDoubleString.VariantGroupKinds> GetIndividualGroups()
        {
            var groups = new IntDoubleString.VariantGroupKinds[@this.IndividualGroupCount];
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

                Span<IntDoubleString.VariantGroupKinds>
                        groups = stackalloc IntDoubleString.VariantGroupKinds[count]; // Count never exceeds 32
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
                        IntDoubleString.VariantGroupKinds.None => nameof(IntDoubleString.VariantGroupKinds.None),
                        IntDoubleString.VariantGroupKinds.Number => nameof(IntDoubleString.VariantGroupKinds.Number),
                        IntDoubleString.VariantGroupKinds.Integer => nameof(IntDoubleString.VariantGroupKinds.Integer),
                        IntDoubleString.VariantGroupKinds.ReferenceType => nameof(IntDoubleString.VariantGroupKinds
                                                                                                 .ReferenceType),
                        IntDoubleString.VariantGroupKinds.ValueType => nameof(IntDoubleString.VariantGroupKinds
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

public static partial class IntDoubleStringVariantKindsOperations
{
    extension(IntDoubleString.VariantKind @this)
    {
        public static ImmutableArray<IntDoubleString.VariantKind> GetAllValues() =>
        [
                IntDoubleString.VariantKind.Int32,
                IntDoubleString.VariantKind.Double,
                IntDoubleString.VariantKind.String
        ];

        public String Name
        {
            get
            {
                var name = @this switch
                {
                        IntDoubleString.VariantKind.Int32 => nameof(Int32),
                        IntDoubleString.VariantKind.Double => nameof(Double),
                        IntDoubleString.VariantKind.String => nameof(String),
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
                        IntDoubleString.VariantKind.Int32 => typeof(Int32),
                        IntDoubleString.VariantKind.Double => typeof(Double),
                        IntDoubleString.VariantKind.String => typeof(String),
                        _ => throw new InvalidOperationException(
                                "Unable to determine type, as the variant was not initialized correctly.")
                };

                return type;
            }
        }

        public IntDoubleString.VariantGroupKinds Groups
        {
            get
            {
                var groups = @this switch
                {
                        IntDoubleString.VariantKind.Int32 => IntDoubleString.VariantGroupKinds.Number |
                                                             IntDoubleString.VariantGroupKinds.Integer |
                                                             IntDoubleString.VariantGroupKinds.ValueType,
                        IntDoubleString.VariantKind.Double => IntDoubleString.VariantGroupKinds.Number |
                                                              IntDoubleString.VariantGroupKinds.ValueType,
                        IntDoubleString.VariantKind.String => IntDoubleString.VariantGroupKinds.ReferenceType,
                        _ => throw new InvalidOperationException(
                                "Unable to determine groups, as the variant was not initialized correctly.")
                };

                return groups;
            }
        }
    }
}
