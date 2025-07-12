// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;

using System.Collections.Generic;

public class ListLikeTests : TestBase
{
    private static readonly String[][] _listLikeTypes =
        new String[][]
        {
            ["ISet", "HashSet"],
            ["IReadOnlySet", "HashSet"],
            ["ICollection", "List"],
            ["IReadOnlyCollection", "List"],
            ["IList", "List"],
            ["IReadOnlyList", "List"],
            ["IEnumerable", "List"],
            ["List", "List"],
            ["HashSet" , "HashSet"]
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
    }.SelectMany(a => _listLikeTypes
        .Select(l => new[] { $"{l[0]}<{a[0]}>", $"{l[1]}<{a[0]}>()", a[1] })
        .Append([$"{a[0]}[]", $"{a[0]}[0]", a[1]]))
     .ToArray();
    public static Object[][] ObjectData =>
   new String[] {
        "global::System.Object",
   }.SelectMany(a => _listLikeTypes
       .Select(l => new[] { $"{l[0]}<{a}>", $"{l[1]}<{a}>()" })
       .Append([$"{a}[]", $"{a}[0]"]))
    .ToArray();

    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_ArrayWithItemTypeForListLikeType(String listLikeType, String initializationType, String expectedType)
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{listLikeType}} Prop { get; set; } = new {{initializationType}};
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "array" },
                        items = new { type = new[] { expectedType } }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(ObjectData))]
    public void Generates_ArrayWithItemTypeForListLikeTypeWithObjectItemType(String listLikeType, String initializationType)
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{listLikeType}} Prop { get; set; } = new {{initializationType}};
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "array" },
                        items = new
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