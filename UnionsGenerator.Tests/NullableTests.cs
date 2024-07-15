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

            {{String.Join('\n', classNames.Select(n => $"class {n} {{}}"))}}
            {{String.Join('\n', structNames.Select(n => $"struct {n} {{}}"))}}

            [UnionType<{{String.Join(',', classNames.Concat(structNames))}}>]
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
                        "global::System.Diagnostics.CodeAnalysis.MemberNotNullWhen")
                    .Select(a => a.ConstructorArguments)
                    .Select(args =>
                        args.Length == expectedMemberNames.Length + 1
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
                            } && actualMemberName == t.Second))
                    .SingleOrDefault();

                Assert.True(hasMatchingAttribute);
            },
            "Union");
    }
}
