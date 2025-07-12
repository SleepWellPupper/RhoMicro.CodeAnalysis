// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.Library.Collections;
using System;

using RhoMicro.CodeAnalysis.Library.Models.Collections;

public class LazyEquatableDictionaryTests
{
    public static TheoryData<Dictionary<Int32, String>> DictionaryValues => new(
        new Dictionary<Int32, String> { { 1, "One" }, { 2, "Two" }, { 3, "Three" } },
        new Dictionary<Int32, String> { { 10, "Ten" }, { 20, "Twenty" }, { 30, "Thirty" } },
        new Dictionary<Int32, String> { { -1, "Negative One" }, { -2, "Negative Two" }, { -3, "Negative Three" } },
        new Dictionary<Int32, String> { { 100, "Hundred" } },
        []
    );

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void ValueSemantics_VerifyEquality(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict1 = factory.CreateLazyDictionary<Int32, String>();
        var dict2 = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict1.Add(kvp.Key, kvp.Value);
            dict2.Add(kvp.Key, kvp.Value);
        }

        // Act & Assert
        Assert.Equal(dict1, dict2); // Dictionaries with identical elements should be equal
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, new[] { "One", "Two", "Three" }, new[] { 1, 2, 3, 4 }, new[] { "One", "Two", "Three", "Four" })]
    [InlineData(new[] { 10, 20, 30 }, new[] { "Ten", "Twenty", "Thirty" }, new[] { 10, 20, 40 }, new[] { "Ten", "Twenty", "Forty" })]
    [InlineData(new[] { -1, -2, -3 }, new[] { "Negative One", "Negative Two", "Negative Three" }, new[] { -1, -2, -4 }, new[] { "Negative One", "Negative Two", "Negative Four" })]
    public void ValueSemantics_VerifyInequality(Int32[] initialKeys, String[] initialValues, Int32[] modifiedKeys, String[] modifiedValues)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict1 = factory.CreateLazyDictionary<Int32, String>();
        var dict2 = factory.CreateLazyDictionary<Int32, String>();

        for(var i = 0; i < initialKeys.Length; i++)
        {
            dict1.Add(initialKeys[i], initialValues[i]);
        }

        for(var i = 0; i < modifiedKeys.Length; i++)
        {
            dict2.Add(modifiedKeys[i], modifiedValues[i]);
        }

        // Act & Assert
        Assert.NotEqual(dict1, dict2); // Dictionaries with different elements should not be equal
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Immutability_AddThrowsAfterSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Act
        dict.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => dict.Add(99, "Ninety-Nine"));
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Immutability_ClearThrowsAfterSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Act
        dict.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(dict.Clear);
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Immutability_RemoveThrowsAfterSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Act
        dict.MutabilityContext.SetImmutable();

        // Assert
        _ = Assert.Throws<InvalidOperationException>(() => dict.Remove(1));
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Mutability_AddSucceedsBeforeSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        // Act
        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Assert
        foreach(var kvp in values)
        {
            Assert.True(dict.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, dict[kvp.Key]);
        }
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Mutability_ClearSucceedsBeforeSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Act
        dict.Clear();

        // Assert
        Assert.Empty(dict);
    }

    [Theory]
    [MemberData(nameof(DictionaryValues))]
    public void Mutability_RemoveSucceedsBeforeSetImmutable(Dictionary<Int32, String> values)
    {
        // Arrange
        var factory = EquatableCollectionFactory.CreateDefault();
        var dict = factory.CreateLazyDictionary<Int32, String>();

        foreach(var kvp in values)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        // Act
        if(values.Count > 0)
        {
            var firstKey = new List<Int32>(values.Keys)[0];
            _ = dict.Remove(firstKey);

            // Assert
            Assert.False(dict.ContainsKey(firstKey));
        } else
        {
            // Assert
            Assert.Empty(dict);
        }
    }
}
