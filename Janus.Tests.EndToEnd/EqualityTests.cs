// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using RhoMicro.CodeAnalysis;
using System;

public partial class EqualityTests
{
    [UnionType<Int32>]
    private sealed partial class Foo;

    [Fact]
    public void EqualisIsValueEquality()
    {
        Foo expected = 32;
        Foo actual = 32;
        Assert.Equal(expected, actual);
    }
}
