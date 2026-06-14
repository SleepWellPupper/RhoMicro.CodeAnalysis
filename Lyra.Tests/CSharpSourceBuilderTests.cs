namespace Lyra.Tests;

using RhoMicro.CodeAnalysis.Lyra;

public class CSharpSourceBuilderTests
{
    private static readonly CSharpSourceBuilderOptions _options = new() { Prelude = static (_, _) => { } };

    [Theory]
    [InlineData(typeof(String), "string")]
    [InlineData(typeof(Boolean), "bool")]
    [InlineData(typeof(SByte), "sbyte")]
    [InlineData(typeof(Int16), "short")]
    [InlineData(typeof(Int32), "int")]
    [InlineData(typeof(Int64), "long")]
    [InlineData(typeof(Byte), "byte")]
    [InlineData(typeof(UInt16), "ushort")]
    [InlineData(typeof(UInt32), "uint")]
    [InlineData(typeof(UInt64), "ulong")]
    [InlineData(typeof(Double), "double")]
    [InlineData(typeof(Decimal), "decimal")]
    [InlineData(typeof(Char), "char")]
    [InlineData(typeof(Object), "object")]
    [InlineData(typeof(IEnumerable<KeyValuePair<String, Object>>), "global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<string, object>>")]
    public void EmitsPrettyType(Type type, String expected)
    {
        using var builder = new CSharpSourceBuilder(_options);
        var actual = builder.AppendTypeName(type).ToString();
        Assert.Equal(expected, actual);
    }
}
