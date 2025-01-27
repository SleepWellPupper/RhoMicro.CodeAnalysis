#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Collections;
using System;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

public class EquatableListTests
{
    public static TheoryData<Int32[]> ListValues =>
        new(
            [1, 2, 3],
            [10, 20, 30],
            [-1, -2, -3],
            [100],
            []
        );

    [Theory]
    [MemberData(nameof(ListValues))]
    public void ValueSemantics_VerifyEquality(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list1 = factory.CreateList<Int32>();
        var list2 = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list1.Add(value);
            list2.Add(value);
        }

        // Act & Assert
        Assert.Equal(list1, list2); // Lists with identical elements should be equal
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
        var list1 = factory.CreateList<Int32>();
        var list2 = factory.CreateList<Int32>();

        foreach(var value in initialValues)
        {
            list1.Add(value);
        }

        foreach(var value in modifiedValues)
        {
            list2.Add(value);
        }

        // Act & Assert
        Assert.NotEqual(list1, list2); // Lists with different elements should not be equal
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Immutability_AddThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => list.Add(3)); // Adding should throw an exception
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Immutability_RemoveThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => list.Remove(values.Length > 0 ? values[0] : 0)); // Removing should throw an exception
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Immutability_ClearThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(list.Clear); // Clearing should throw an exception
    }
    [Theory]
    [MemberData(nameof(ListValues))]
    public void Immutability_InsertThrowsAfterSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => list.Insert(0, 999)); // Inserting should throw an exception
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Immutability_RemoveAtThrowsAfterSetImmutable(Int32[] values)
    {
        if(values.Length == 0)
            return;

        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => list.RemoveAt(0)); // Removing at index should throw an exception
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Mutability_AddSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        // Act
        foreach(var value in values)
        {
            list.Add(value);
        }

        // Assert
        foreach(var value in values)
        {
            Assert.Contains(value, list); // List should contain the added elements
        }
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Mutability_RemoveSucceedsBeforeSetImmutable(Int32[] values)
    {
        if(values.Length == 0)
            return;

        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        _ = list.Remove(values[0]);

        // Assert
        Assert.DoesNotContain(values[0], list); // List should not contain the removed element
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Mutability_ClearSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.Clear();

        // Assert
        Assert.Empty(list); // List should be empty after clearing
    }
    [Theory]
    [MemberData(nameof(ListValues))]
    public void Mutability_InsertSucceedsBeforeSetImmutable(Int32[] values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.Insert(0, 999);

        // Assert
        Assert.Equal(999, list[0]); // The inserted element should be at the correct position
    }

    [Theory]
    [MemberData(nameof(ListValues))]
    public void Mutability_RemoveAtSucceedsBeforeSetImmutable(Int32[] values)
    {
        if(values.Length == 0)
            return;

        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var list = factory.CreateList<Int32>();

        foreach(var value in values)
        {
            list.Add(value);
        }

        // Act
        list.RemoveAt(0);

        // Assert
        Assert.DoesNotContain(values[0], list); // List should not contain the removed element
    }
}
