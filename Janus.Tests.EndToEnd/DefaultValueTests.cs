// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

#pragma warning disable CS1591
public sealed partial class DefaultValueTests
{
    [UnionType<String, List<Int32>>]
    partial struct StructUnionWithUnknownVariant;

    [Fact]
    public void StructUnionWithoutValueTypeVariantThrowsUnknownVariant()
    {
        Assert.Throws<InvalidOperationException>(() =>
            default(StructUnionWithUnknownVariant).Switch(
                onList: _ => Assert.Fail(),
                onString: _ => Assert.Fail()));
    }

    [UnionType<String, List<Int32>, DateTime>]
    partial struct StructUnionWithoutUnknownVariant;

    [Fact]
    public void StructUnionWithValueTypeVariantInitializesToValueTypeVariant()
    {
        default(StructUnionWithoutUnknownVariant).Switch(
            onList: _ => Assert.Fail(),
            onString: _ => Assert.Fail(),
            onDateTime: _ => { });
    }

    [UnionType<Int32>(IsDefault = true)]
    [UnionType<List<Int32>>(IsNullable = true)]
    // unmanaged but alphabetically first, managed value type, non-nullable reference type
    [UnionType<Double, CancellationToken, String>]
    partial struct StructUnionWithUnmanagedDefaultVariant;

    [Fact]
    public void StructUnionWithUnmanagedDefaultVariantInitializesToDefault()
    {
        StructUnionWithUnmanagedDefaultVariant u = default;
        Assert.True(u.IsInt32, $"Actual variant was `{u.Variant.Name}`.");
    }

    [UnionType<CancellationToken>(IsDefault = true)]
    [UnionType<List<Int32>>(IsNullable = true)]
    // unmanaged, non-nullable reference type
    [UnionType<Double, String>]
    partial struct StructUnionWithManagedStructDefaultVariant;

    [Fact]
    public void StructUnionWithManagedStructDefaultVariantInitializesToDefault()
    {
        StructUnionWithManagedStructDefaultVariant u = default;
        Assert.True(u.IsCancellationToken, $"Actual variant was `{u.Variant.Name}`.");
    }

    [UnionType<List<Int32>>(IsNullable = true, IsDefault = true)]
    // unmanaged, managed struct, non-nullable reference type
    [UnionType<Double, CancellationToken, String>]
    partial struct StructUnionWithNullableReferenceTypeDefaultVariant;

    [Fact]
    public void StructUnionWithNullableReferenceTypeDefaultVariantInitializesToDefault()
    {
        StructUnionWithNullableReferenceTypeDefaultVariant u = default;
        Assert.True(u.IsList, $"Actual variant was `{u.Variant.Name}`.");
    }
}
