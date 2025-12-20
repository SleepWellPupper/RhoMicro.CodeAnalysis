// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class NullableValueTypesTests : TestBase
{
    [Theory]
    [InlineData("global::System.DateTime", "string")]
    [InlineData("global::System.TimeSpan", "string")]
    [InlineData("global::System.DateOnly", "string")]
    [InlineData("global::System.TimeOnly", "string")]
    [InlineData("global::System.DateTimeOffset", "string")]

    [InlineData("global::System.Single", "number")]
    [InlineData("global::System.Double", "number")]
    [InlineData("global::System.Decimal", "number")]

    [InlineData("global::System.Byte", "integer")]
    [InlineData("global::System.SByte", "integer")]
    [InlineData("global::System.Int16", "integer")]
    [InlineData("global::System.UInt16", "integer")]
    [InlineData("global::System.Int32", "integer")]
    [InlineData("global::System.UInt32", "integer")]
    [InlineData("global::System.Int64", "integer")]
    [InlineData("global::System.UInt64", "integer")]

    [InlineData("global::System.Boolean", "boolean")]
    public void GeneratesTypeOrNullForNullableValueType(String valueType, String expectedType)
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public global::System.Nullable<{{valueType}}> Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { expectedType, "null" } }
                },
                ["additionalProperties"] = false
            });
    }
}
