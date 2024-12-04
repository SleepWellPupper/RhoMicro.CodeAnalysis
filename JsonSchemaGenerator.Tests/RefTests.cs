#pragma warning disable CA1861
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class RefTests : TestBase
{
    [Fact]
    public void Generates_RefForCircularDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Schema? Prop { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        oneOf = new Object[]
                        {
                            new Dictionary<String, Object>() { ["$ref"] = "Schema" },
                            new { type = new [] { "null" } }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void Generates_RefForSimpleSchemaDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Dependency Prop { get; set; } = new Dependency();
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency { }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = "Dependency" },
                },
                ["additionalProperties"] = false
            }, id: "Schema");
    }
    [Fact]
    public void Generates_RefForComplexSchemaDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Dependency Prop { get; set; } = new Dependency();
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency 
            {
                public int Prop { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = "Dependency" },
                },
                ["additionalProperties"] = false
            }, id: "Schema");

        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Dependency Prop { get; set; } = new Dependency();
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency 
            {
                public int Prop { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Dependency",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "integer" }
                    },
                },
                ["additionalProperties"] = false
            }, id: "Dependency");
    }
    [Fact]
    public void Generates_RequiredRefForRequiredSchemaDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public required Dependency Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency { }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = "Dependency" },
                },
                ["additionalProperties"] = false,
                ["required"] = new[] { "Prop" }
            }, id: "Schema");
    }
    [Fact]
    public void Generates_NullableRefForSchemaDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Dependency? Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency { }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        oneOf = new Object[]
                        {
                            new Dictionary<String, Object>() { ["$ref"] = "Dependency" },
                            new { type = new[] { "null" } }
                        }
                    },
                },
                ["additionalProperties"] = false,
            }, id: "Schema");
    }
}
