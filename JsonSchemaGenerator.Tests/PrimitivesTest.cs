#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class PrimitivesTest : TestBase
{
    [Theory]
    [InlineData("global::System.Byte")]
    [InlineData("global::System.SByte")]
    [InlineData("global::System.Int16")]
    [InlineData("global::System.UInt16")]
    [InlineData("global::System.Int32")]
    [InlineData("global::System.UInt32")]
    [InlineData("global::System.Int64")]
    [InlineData("global::System.UInt64")]
    public void Generates_IntegerTypeForIntegralPrimitives(String type)
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{type}} Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "integer" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [InlineData("global::System.Single")]
    [InlineData("global::System.Double")]
    [InlineData("global::System.Decimal")]
    public void Generates_NumberTypeForDecimalPrimitives(String type)
    {

        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{type}} Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "number" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [InlineData("global::System.DateTime")]
    [InlineData("global::System.TimeSpan")]
    [InlineData("global::System.DateOnly")]
    [InlineData("global::System.TimeOnly")]
    [InlineData("global::System.DateTimeOffset")]
    public void Generates_StringTypeForDateTypes(String type)
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public {{type}} Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "string" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_BooleanTypeForBooleanPrimitive()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public bool Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "boolean" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_StringTypeForStringPrimitive()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public string Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new { type = new[] { "string" } }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_NullableObjectTypeForNullableObjectPrimitive()
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
    [Fact]
    public void Generates_ObjectTypeForObjectPrimitive()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public object Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "object" },
                        additionalProperties = false
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_EmptySchemaForEmptyType()
    {
        TestSchema(
            $$"""
                [RhoMicro.CodeAnalysis.JsonSchema]
                class Schema { }
                """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["additionalProperties"] = false
            });
    }
}
