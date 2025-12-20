// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class AnnotationsTests : TestBase
{

    [Fact]
    public void GeneratesDescriptionForSchema()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema(Description = "foobar")]
            class Schema
            {
                public object Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
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
    public void GeneratesTitleForSchema()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema(Title = "foobar")]
            class Schema
            {
                public object Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
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
    public void GeneratesDescriptionForProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Description = "foobar")]
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
                        additionalProperties = false,
                        description = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesTitleForProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Title = "foobar")]
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
                        additionalProperties = false,
                        title = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesTitleForRefProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Title = "foobar")]
                public Dependency Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency;
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>()
                    {
                        ["$ref"] = $"../{n}/Dependency.json",
                        ["title"] = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesDescriptionForRefProperty()
    {
        TestSchema(
            $$"""
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Description = "foobar")]
                public Dependency Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Dependency;
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>()
                    {
                        ["$ref"] = $"../{n}/Dependency.json",
                        ["description"] = "foobar"
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesTitleForEnumProperty()
    {
        TestSchema(
            $$"""
            enum MyEnum
            {
                None,
                One,
                Two
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Title = "foobar")]
                public MyEnum Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        anyOf = new Object[]
                        {
                            new
                            {
                                @enum = new Object[] { "None", "One", "Two", 0, 1, 2},
                                title = "foobar"
                            },
                            new
                            {
                                type= new[]{"integer" },
                                title = "foobar"
                            }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesDescriptionForEnumProperty()
    {
        TestSchema(
            $$"""
            enum MyEnum
            {
                None,
                One,
                Two
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                [RhoMicro.CodeAnalysis.JsonSchemaProperty(Description = "foobar")]
                public MyEnum Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        anyOf = new Object[]
                        {
                            new
                            {
                                @enum = new Object[] { "None", "One", "Two", 0, 1, 2},
                                description = "foobar"
                            },
                            new
                            {
                                type= new[]{"integer" },
                                description = "foobar"
                            }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
}
