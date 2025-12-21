// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using System;

public partial class RelatedTypeConversionTests
{
    [UnionType<DateTime>]
    [UnionType<String>]
    [UnionType<Double>]
    private readonly partial struct Union;

    [UnionType<Double>]
    [UnionType<DateTime>]
    [UnionType<String>]
    private sealed partial class CongruentUnion;

    [Fact]
    public void StringUnionToCongruent()
    {
        Union u = "Hello, World!";
        var cu = CongruentUnion.Create(u);

        Assert.Equal(u.AsString, cu.AsString);
    }

    [Fact]
    public void StringCongruentToUnion()
    {
        CongruentUnion cu = "Hello, World!";
        var u = Union.Create(cu);

        Assert.Equal(cu.AsString, u.AsString);
    }

    [Fact]
    public void DoubleUnionToCongruent()
    {
        Union u = 32d;
        var cu = CongruentUnion.Create(u);

        Assert.Equal(u.AsDouble, cu.AsDouble);
    }

    [Fact]
    public void DoubleCongruentToUnion()
    {
        CongruentUnion cu = 32d;
        var u = Union.Create(cu);

        Assert.Equal(cu.AsDouble, u.AsDouble);
    }

    [Fact]
    public void DateTimeUnionToCongruent()
    {
        Union u = DateTime.Parse("01/10/2009 7:34");
        var cu = CongruentUnion.Create(u);

        Assert.Equal(u.AsDateTime, cu.AsDateTime);
    }

    [Fact]
    public void DateTimeCongruentToUnion()
    {
        CongruentUnion cu = DateTime.Parse("01/10/2009 7:34");
        var u = Union.Create(cu);

        Assert.Equal(cu.AsDateTime, u.AsDateTime);
    }

    [UnionType<DateTime>]
    [UnionType<String>]
    private sealed partial class SubsetUnion;

    [Fact]
    public void StringUnionToSubset()
    {
        Union u = "Hello, World!";
        var su = SubsetUnion.Create(u);

        Assert.Equal(u.AsString, su.AsString);
    }

    [Fact]
    public void StringSubsetToUnion()
    {
        SubsetUnion su = "Hello, World!";
        var u = Union.Create(su);

        Assert.Equal(su.AsString, u.AsString);
    }

    [Fact]
    public void DoubleUnionToSubset()
    {
        Union u = 32d;
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => SubsetUnion.Create(u));
    }

    [Fact]
    public void DateTimeUnionToSubset()
    {
        Union u = DateTime.Parse("01/10/2009 7:34");
        var su = SubsetUnion.Create(u);

        Assert.Equal(u.AsDateTime, su.AsDateTime);
    }

    [Fact]
    public void DateTimeSubsetToUnion()
    {
        SubsetUnion su = DateTime.Parse("01/10/2009 7:34");
        var u = Union.Create(su);

        Assert.Equal(su.AsDateTime, u.AsDateTime);
    }

    [UnionType<DateTime>]
    [UnionType<String>]
    [UnionType<Double>]
    [UnionType<Int32>]
    private readonly partial struct SupersetUnion;

    [Fact]
    public void StringUnionToSuperset()
    {
        Union u = "Hello, World!";
        var su = SupersetUnion.Create(u);

        Assert.Equal(u.AsString, su.AsString);
    }

    [Fact]
    public void StringSupersetToUnion()
    {
        SupersetUnion su = "Hello, World!";
        var u = Union.Create(su);

        Assert.Equal(su.AsString, u.AsString);
    }

    [Fact]
    public void Int32SupersetToUnion()
    {
        SupersetUnion su = 32;
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Union.Create(su));
    }

    [Fact]
    public void DateTimeUnionToSuperset()
    {
        Union u = DateTime.Parse("01/10/2009 7:34");
        var su = SupersetUnion.Create(u);

        Assert.Equal(u.AsDateTime, su.AsDateTime);
    }

    [Fact]
    public void DateTimeSupersetToUnion()
    {
        SupersetUnion su = DateTime.Parse("01/10/2009 7:34");
        var u = Union.Create(su);

        Assert.Equal(su.AsDateTime, u.AsDateTime);
    }

    [Fact]
    public void DoubleUnionToSuperset()
    {
        Union u = 32d;
        var su = SupersetUnion.Create(u);

        Assert.Equal(u.AsDouble, su.AsDouble);
    }

    [Fact]
    public void DoubleSupersetToUnion()
    {
        SupersetUnion su = 32d;
        var u = Union.Create(su);

        Assert.Equal(su.AsDouble, u.AsDouble);
    }

    [UnionType<Int16>]
    [UnionType<String>]
    [UnionType<Double>]
    [UnionType<List<Byte>>]
    private sealed partial class IntersectionUnion;

    [Fact]
    public void Int16IntersectionToUnion()
    {
        IntersectionUnion iu = 32;
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Union.Create(iu));
    }

    [Fact]
    public void ListIntersectionToUnion()
    {
        IntersectionUnion iu = new List<Byte>();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Union.Create(iu));
    }

    [Fact]
    public void DateTimeUnionToIntersection()
    {
        Union iu = DateTime.Parse("01/10/2009 7:34");
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => IntersectionUnion.Create(iu));
    }

    [Fact]
    public void StringUnionToIntersection()
    {
        Union u = "Hello, World!";
        var iu = IntersectionUnion.Create(u);

        Assert.Equal(u.AsString, iu.AsString);
    }

    [Fact]
    public void StringIntersectionToUnion()
    {
        IntersectionUnion iu = "Hello, World!";
        var u = Union.Create(iu);

        Assert.Equal(iu.AsString, u.AsString);
    }

    [Fact]
    public void DoubleUnionToIntersection()
    {
        Union u = 32d;
        var iu = IntersectionUnion.Create(u);

        Assert.Equal(u.AsDouble, iu.AsDouble);
    }

    [Fact]
    public void DoubleIntersectionToUnion()
    {
        IntersectionUnion iu = 32d;
        var u = Union.Create(iu);

        Assert.Equal(iu.AsDouble, u.AsDouble);
    }
}
