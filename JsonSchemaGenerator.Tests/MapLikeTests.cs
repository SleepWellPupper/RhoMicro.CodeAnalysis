#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;

using System.Collections.Generic;

public class MapLikeTests : TestBase
{
    private static readonly String[][] _mapLikeTypes =
        new String[][]
        {
            ["IDictionary", "Dictionary"],
            ["IReadOnlyDictionary", "Dictionary"],
            ["Dictionary", "Dictionary"],
        }.Select(a => a.Select(n => $"global::System.Collections.Generic.{n}").ToArray())
        .ToArray();
    public static Object[][] Data =>
    new String[][] {
        ["global::System.DateTime", "string"],
        ["global::System.TimeSpan", "string"],
        ["global::System.DateOnly", "string"],
        ["global::System.TimeOnly", "string"],
        ["global::System.DateTimeOffset", "string"],

        ["global::System.Single", "number"],
        ["global::System.Double", "number"],
        ["global::System.Decimal", "number"],

        ["global::System.Byte", "integer"],
        ["global::System.SByte", "integer"],
        ["global::System.Int16", "integer"],
        ["global::System.UInt16", "integer"],
        ["global::System.Int32", "integer"],
        ["global::System.UInt32", "integer"],
        ["global::System.Int64", "integer"],
        ["global::System.UInt64", "integer"],

        ["global::System.Boolean", "boolean"],

        ["global::System.String", "string"]
    }.SelectMany(a => _mapLikeTypes.Select(l => new[] { $"{l[0]}<string, {a[0]}>", $"{l[1]}<string, {a[0]}>()", a[1] }))
     .ToArray();
    public static Object[][] ObjectData =>
    new String[] {
        "global::System.Object",
    }.SelectMany(a => _mapLikeTypes.Select(l => new[] { $"{l[0]}<string, {a}>", $"{l[1]}<string, {a}>()" }))
     .ToArray();

    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_ObjectTypeForMapLikeType(String mapLikeType, String initializationType, String expectedType)
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{mapLikeType}} Prop { get; set; } = new {{initializationType}};
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "object" },
                        additionalProperties = new
                        {
                            type = new[] { expectedType }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(ObjectData))]
    public void Generates_ObjectTypeForMapLikeTypeWithObjectValueType(String mapLikeType, String initializationType)
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{mapLikeType}} Prop { get; set; } = new {{initializationType}};
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "object" },
                        additionalProperties = new
                        {
                            type = new[] { "object" },
                            additionalProperties = false
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
}
