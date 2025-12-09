// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;
using System;
using System.Collections.Immutable;

public partial class ToStringTests
{
    [UnionType<String>]
    [UnionTypeSettings(ToStringSetting = ToStringSetting.None)]
    private sealed partial class NoToStringUnionType;

    [Fact]
    public void UsesDefaultToString()
    {
        NoToStringUnionType u = "Foo";
        var expected = typeof(NoToStringUnionType).FullName;
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }

    [UnionType<String, Int32>]
    [UnionTypeSettings(ToStringSetting = ToStringSetting.Simple)]
    private sealed partial class SimpleToStringUnionType;
    [Fact]
    public void UsesSimpleToString()
    {
        const String expected = "Foo";
        SimpleToStringUnionType u = expected;
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }

    [UnionType<double, int, ImmutableArray<byte>, string>]
    [UnionTypeSettings(JsonConverterSetting = JsonConverterSetting.EmitJsonConverter)]
    sealed partial class IntDoubleString;

    [Fact]
    public void UsesDetailedToString2()
    {
        const String expected = "IntDoubleString { Variants: [Double, <Int32>, ImmutableArray, String], Value: 42 }";
        IntDoubleString u = 42;
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }
    
    [UnionType<String, Int32>]
    [UnionTypeSettings(ToStringSetting = ToStringSetting.Detailed)]
    private sealed partial class DetailedToStringUnionType;

    [Fact]
    public void UsesDetailedToString1()
    {
        const String expected = "DetailedToStringUnionType { Variants: [Int32, <String>], Value: Foo }";
        DetailedToStringUnionType u = "Foo";
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }
    
    [UnionType<String>]
    private sealed partial class CustomToStringUnionType
    {
        public override String ToString() => "Foo";
    }

    [Fact]
    public void UsesCustomToString()
    {
        CustomToStringUnionType u = "Bar";
        var expected = "Foo";
        var actual = u.ToString();

        Assert.Equal(expected, actual);
    }
}
