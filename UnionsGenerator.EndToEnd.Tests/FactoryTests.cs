namespace RhoMicro.CodeAnalysis.UnionsGenerator.EndToEnd.Tests;
using System;

public partial class FactoryTests
{
    [UnionType<Int32>(FactoryName = "MyFactory")]
    partial class NamedFactoryUnion;

    [Fact]
    public void UsesProvidedFactoryName()
    {
        _ = NamedFactoryUnion.MyFactory(0);
    }

    [UnionType<Int32>]
    partial class UnnamedFactoryUnion;

    [Fact]
    public void UsesDefaultFactoryName()
    {
        _ = UnnamedFactoryUnion.Create(0);
    }
}
