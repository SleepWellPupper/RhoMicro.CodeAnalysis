namespace RhoMicro.CodeAnalysis.UnionsGenerator.EndToEnd.Tests;

using RhoMicro.CodeAnalysis;

using System;

public partial class EqualityTests
{
    [UnionType<Int32>]
    private partial class Foo
    {
        public Boolean Equals(Foo? foo) => true;
    }
    [UnionType<Int32>]
    private partial class Bar;
}
