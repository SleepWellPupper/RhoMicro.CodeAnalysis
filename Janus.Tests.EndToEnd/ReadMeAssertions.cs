// SPDX-License-Identifier: MPL-2.0

#pragma warning disable IDE0250
#pragma warning disable IDE0059
#pragma warning disable CS1591
#pragma warning disable RMJ0021

namespace RhoMicro.CodeAnalysis.Janus.EndToEnd.Tests;

using System;
using System.Collections.Generic;

public partial class ReadMeAssertions
{
    [UnionType<Int32>]
    [UnionType<String>]
    private partial struct IntOrString;

    [Fact]
    public void TypeDeclarationTarget()
    {
        IntOrString u = "Hello, World!"; //implicitly converted
        u = 32; //implicitly converted
    }

    private partial struct GenericUnion<[UnionType] T0, [UnionType] T1>;

    [Fact]
    public void TypeParameterTarget()
    {
        var u = GenericUnion<Int32, String>.Create("Hello, World!");
        u = GenericUnion<Int32, String>.Create(32);
    }

    [UnionType<List<String>>(Name = "MultipleNames")]
    [UnionType<String>(Name = "SingleName")]
    private sealed partial class Names;

    [Fact]
    public void AliasExample()
    {
        Names n = "John";
        if (n.IsSingleName)
        {
            var singleName = n.AsSingleName;
        }
        else if (n.IsMultipleNames)
        {
            var multipleNames = n.AsMultipleNames;
        }
    }

    [UnionType<Int32>]
    private partial struct Int32Alias;

    [Fact]
    public void Solitary()
    {
        var i = 32;
        Int32Alias u = i;
        i = u;
    }

    private partial struct GenericConvertableUnion<[UnionType] T>;

    [Fact]
    public void SupersetOfParameter()
    {
        GenericConvertableUnion<Int32> u = 32;
        var i = u;
    }

#pragma warning disable CS8604 // Possible null reference argument.
    [UnionType<String>(IsNullable = true)]
    [UnionType<List<String>>]
    private partial struct NullableStringUnion;

    [Fact]
    public void NullableUnion()
    {
        NullableStringUnion u = (String?)null;
        u = new List<String>();
        u = "Nonnull String";
        u = (List<String>?)null; //CS8604 - Possible null reference argument for parameter.
    }
#pragma warning restore CS8604 // Possible null reference argument.

    [UnionType<Int32, Single>(Groups = ["Number"])]
    [UnionType<String, Char>(Groups = ["Text"])]
    private partial struct GroupedUnion;

    [Fact]
    public void GroupedUnions()
    {
        GroupedUnion u = "Hello, World!";
        if (u.Variant.Group.ContainsNumber)
        {
            Assert.Fail("Expected union to be text.");
        }

        if (!u.Variant.Group.ContainsText)
        {
            Assert.Fail("Expected union to be text.");
        }

        u = 32f;
        if (!u.Variant.Group.ContainsNumber)
        {
            Assert.Fail("Expected union to be number.");
        }

        if (u.Variant.Group.ContainsText)
        {
            Assert.Fail("Expected union to be number.");
        }
    }
}
