// SPDX-License-Identifier: MPL-2.0

#pragma warning disable CA1861
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class RefTests : TestBase
{
    [Fact]
    public void GeneratesRefForCircularDependency()
    {
        TestSchema(
            $$"""
            #nullable enable
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public Schema? Prop { get; set; }
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
                            new Dictionary<String, Object>() { ["$ref"] = $"../{n}/Schema.json" },
                            new { type = new [] { "null" } }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Fact]
    public void GeneratesRefForSimpleSchemaDependency()
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
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = $"../{n}/Dependency.json", },
                },
                ["additionalProperties"] = false
            }, n => $"./{n}/Schema.json");
    }
    [Fact]
    public void GeneratesRefForComplexSchemaDependency()
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
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = $"../{n}/Dependency.json", },
                },
                ["additionalProperties"] = false
            }, n => $"./{n}/Schema.json");

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
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Dependency.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new
                    {
                        type = new[] { "integer" }
                    },
                },
                ["additionalProperties"] = false
            }, n => $"./{n}/Dependency.json");
    }
    [Fact]
    public void GeneratesRequiredRefForRequiredSchemaDependency()
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
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>() { ["$ref"] = $"../{n}/Dependency.json", },
                },
                ["additionalProperties"] = false,
                ["required"] = new[] { "Prop" }
            }, n => $"./{n}/Schema.json");
    }
    [Fact]
    public void GeneratesNullableRefForSchemaDependency()
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
                            new Dictionary<String, Object>() { ["$ref"] = $"../{n}/Dependency.json", },
                            new { type = new[] { "null" } }
                        }
                    },
                },
                ["additionalProperties"] = false,
            }, n => $"./{n}/Schema.json");
    }
}
