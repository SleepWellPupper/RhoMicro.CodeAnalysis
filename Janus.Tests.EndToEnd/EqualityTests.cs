// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using RhoMicro.CodeAnalysis;
using System;

public partial class EqualityTests
{
    [UnionType<Int32>]
    private sealed partial class Foo;

    [UnionType<Int32>]
    private readonly partial struct StructUnion;

    [Fact]
    public void EqualsIsValueEquality()
    {
        Foo expected = 32;
        Foo actual = 32;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void StructUnionHasEqualityOperator()
    {
        StructUnion expected = 32;
        StructUnion actual = 32;
        Assert.True(expected == actual);
    }
}
