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
}
