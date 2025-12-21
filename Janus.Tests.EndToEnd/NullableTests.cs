// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using System;

public partial class NullableTests
{
    [UnionType<Boolean?>]
    private readonly partial struct NullableBoolUnion;

    [Fact]
    public void NullableBoolTrueFactoryCall()
    {
        var u = NullableBoolUnion.Create((Boolean?)true);
        Assert.True(u.IsBoolean);
        Assert.True(u.AsBoolean.HasValue);
        Assert.True(u.AsBoolean.Value);
    }

    [Fact]
    public void NullableBoolFalseFactoryCall()
    {
        var u = NullableBoolUnion.Create((Boolean?)false);
        Assert.True(u.IsBoolean);
        Assert.True(u.AsBoolean.HasValue);
        Assert.False(u.AsBoolean.Value);
    }

    [Fact]
    public void NullableBoolNullFactoryCall()
    {
        var u = NullableBoolUnion.Create((Boolean?)null);
        Assert.True(u.IsBoolean);
        Assert.False(u.AsBoolean.HasValue);
    }
}
