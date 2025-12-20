// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Collections;
using System;
using System.Collections.Generic;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

public class EquatableSetTests
{
    public static TheoryData<Int32[]> SetValues => new(
        [1, 2, 3],
        [10, 20, 30],
        [-1, -2, -3],
        [100],
        []
    );

    [Theory]
    [MemberData(nameof(SetValues))]
    public void ValueSemantics_VerifyEquality(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set1 = factory.CreateSet<Int32>();
        var set2 = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set1.Add(value);
            _ = set2.Add(value);
        }

        // Act & Assert
        Assert.Equal(set1, set2); // Sets with identical elements should be equal
    }

    [Theory]
    [InlineData(new Int32[] { 1, 2, 3 }, new Int32[] { 1, 2, 3, 999 })]
    [InlineData(new Int32[] { 10, 20, 30 }, new Int32[] { 10, 20, 9999 })]
    [InlineData(new Int32[] { -1, -2, -3 }, new Int32[] { -1, -2, -3, 0 })]
    [InlineData(new Int32[] { 100 }, new Int32[] { 100, 101 })]
    [InlineData(new Int32[] { }, new Int32[] { 999 })]
    public void ValueSemantics_VerifyInequality(Int32[] initialValues, Int32[] modifiedValues)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set1 = factory.CreateSet<Int32>();
        var set2 = factory.CreateSet<Int32>();

        foreach(var value in initialValues)
        {
            _ = set1.Add(value);
        }

        foreach(var value in modifiedValues)
        {
            _ = set2.Add(value);
        }

        // Act & Assert
        Assert.NotEqual(set1, set2); // Sets with different elements should not be equal
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_AddThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => set.Add(3)); // Adding should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_ClearThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(set.Clear); // Clearing should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_ExceptWithThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => set.ExceptWith([1])); // ExceptWith should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_IntersectWithThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => set.IntersectWith([1])); // IntersectWith should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_SymmetricExceptWithThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => set.SymmetricExceptWith([1])); // SymmetricExceptWith should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Immutability_UnionWithThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => set.UnionWith([1])); // UnionWith should throw an exception
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_AddSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        // Act
        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Assert
        foreach(var value in values)
        {
            Assert.Contains(value, set); // Set should contain the added elements
        }
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_ClearSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.Clear();

        // Assert
        Assert.Empty(set); // Set should be empty after clearing
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_ExceptWithSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.ExceptWith([1]);

        // Assert
        Assert.DoesNotContain(1, set); // Set should not contain the excluded element
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_IntersectWithSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.IntersectWith([1]);

        // Assert
        Assert.All(set, item => Assert.Equal(1, item)); // Set should only contain intersected elements
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_SymmetricExceptWithSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.SymmetricExceptWith([1]);

        // Assert
        var expected = new HashSet<Int32>(values);
        expected.SymmetricExceptWith([1]);
        Assert.Equal(expected, set); // Set should now contain the symmetric difference
    }

    [Theory]
    [MemberData(nameof(SetValues))]
    public void Mutability_UnionWithSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var set = factory.CreateSet<Int32>();

        foreach(var value in values)
        {
            _ = set.Add(value);
        }

        // Act
        set.UnionWith([1]);

        // Assert
        Assert.Contains(1, set); // Set should contain all elements in the union
    }
}
