// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class NullableReferenceTypeTests : TestBase
{
    [Fact]
    public void Generates_StringOrNullForNullableString()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public string? Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "string", "null" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_ObjectOrNullForNullableObject()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public object? Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "object", "null" },
                        additionalProperties = false
                    }
                },
                ["additionalProperties"] = false
            });
    }
}
