namespace UtilityGenerators.Tests;

using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.UtilityGenerators.Tests;

public class TypeCheckTests : TestBase<AttributeFactoryGenerator>
{
    [Fact]
    public void Test1()
    {
        base.TestFactory(
            """
            using System;
            partial class Foo : Attribute;
            """,
            """

            """)
    }
}
