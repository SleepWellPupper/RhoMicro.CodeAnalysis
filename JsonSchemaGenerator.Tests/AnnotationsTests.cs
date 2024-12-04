#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class AnnotationsTests : TestBase
{

    [Fact]
    public void Generates_DescriptionForSchema()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema(Description = "foobar")]
            class Schema
            {
                public object Prop { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["description"] = "foobar",
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
    public void Generates_TitleForSchema()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema(Title = "foobar")]
            class Schema
            {
                public object Prop { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["title"] = "foobar",
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
    public void Generates_DescriptionForProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Description = "foobar")]
                public object Prop { get; set; }
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
                        additionalProperties = false,
                        description = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_TitleForProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Title = "foobar")]
                public object Prop { get; set; }
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
                        additionalProperties = false,
                        title = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
}