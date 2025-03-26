#pragma warning disable CS1591

namespace RhoMicro.CodeAnalysis.UtilityGenerators.Tests.E2E.Library.Models;

using RhoMicro.CodeAnalysis.Library.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NamedTypeModelTests
{
    [Theory]
    [InlineData(
        new[] { "Foo" },
        "Bar",
        new[] { "Baz" },
        "Foo.Bar.1.g.cs")]
    [InlineData(
        new[] { "Foo", "Bar" },
        "Baz",
        new[] { "FooBar" },
        "Foo.Bar.Baz.1.g.cs")]
    [InlineData(
        new[] { "Foo", "Bar" },
        "Baz",
        new String[] { },
        "Foo.Bar.Baz.g.cs")]
    [InlineData(
        new String[] { },
        "Bar",
        new[] { "Baz" },
        "Bar.1.g.cs")]
    [InlineData(
        new String[] { },
        "Baz",
        new[] { "FooBar" },
        "Baz.1.g.cs")]
    [InlineData(
        new String[] { },
        "Baz",
        new String[] { },
        "Baz.g.cs")]
    public void NonNestedTypeBuildsExpectedHintName(
        String[] namespaceParts,
        String name,
        String[] typeArguments,
        String expected)
    {
        var actual = new NamedTypeModel(
            ContainingTypes: [],
            Accessibility: null,
            Kind: PartialTypeKindModel.Class,
            TypeArguments: [.. typeArguments.Select(a=>new TypeModel(
                NamespaceParts:[..namespaceParts],
                Name:a))],
            NamespaceParts: [.. namespaceParts],
            Name:
        name).GetHintName(CancellationToken.None);

        Assert.Equal(expected, actual);
    }
}
