namespace RhoMicro.CodeAnalysis.UnionsGenerator.Tests;
using System;
using System.Linq;

using Microsoft.CodeAnalysis;

public class NullableTests : TestBase
{
    [Fact]
    public void AllowsForNullableValueType()
    {
        TestUnionType(
            """
            using System;
            using RhoMicro.CodeAnalysis;

            [UnionType<Nullable<bool>>]
            partial struct NullableBoolUnion { }
            """,
            s => { });
    }

    [Theory]
    [InlineData(new[] { "Foo", "Bar" }, new String[] { }, "IsFoo", true, "AsFoo")]
    [InlineData(new[] { "Foo", "Bar" }, new String[] { }, "IsBar", true, "AsBar")]
    [InlineData(new[] { "Foo", "Bar" }, new String[] { }, "IsFoo", false, "AsBar")]
    [InlineData(new[] { "Foo", "Bar" }, new String[] { }, "IsBar", false, "AsFoo")]

    [InlineData(new[] { "Foo" }, new String[] { "Bar" }, "IsFoo", true, "AsFoo")]
    [InlineData(new[] { "Foo" }, new String[] { "Bar" }, "IsBar", true)]
    [InlineData(new[] { "Foo" }, new String[] { "Bar" }, "IsFoo", false)]
    [InlineData(new[] { "Foo" }, new String[] { "Bar" }, "IsBar", false, "AsFoo")]
    public void AnnotatesWithMemberNotNullWhenAttribute(
        String[] classNames,
        String[] structNames,
        String propertyName,
        Boolean expectedCondition,
        params String[] expectedMemberNames)
    {
        TestUnionType(
            $$"""
            using RhoMicro.CodeAnalysis;

            {{String.Join('\n', classNames.Select(n => $"class {n.TrimEnd('?')} {{}}"))}}
            {{String.Join('\n', structNames.Select(n => $"struct {n} {{}}"))}}

            [UnionType<{{String.Join(',', structNames.Concat(classNames.Select(n=>n.TrimEnd('?'))))}}>]
            partial struct Union { }
            """,
            s =>
            {
                var hasMatchingAttribute = s.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Single(p => p.Name == propertyName)
                    .GetAttributes()
                    .Where(a =>
                        a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
                        "global::System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute")
                    .Select(a => a.ConstructorArguments)
                    .Select(args =>
                    {
                        if(expectedMemberNames.Length == 0)
                            return true;

                        var result = args.Length == expectedMemberNames.Length + 1
                            && args[0] is
                            {
                                Kind: TypedConstantKind.Primitive,
                                Value: Boolean actualCondition
                            }
                            && actualCondition == expectedCondition
                            && args.Skip(1)
                                .Zip(expectedMemberNames)
                                .All(t => t.First is
                                {
                                    Kind: TypedConstantKind.Primitive,
                                    Value: String actualMemberName
                                } && actualMemberName == t.Second);

                        return result;
                    })
                    .Where(v => v)
                    .SingleOrDefault();

                Assert.True(hasMatchingAttribute);
            },
            "Union");
    }
}
