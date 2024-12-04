#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;
public class EnumTests : TestBase
{
    public static Object[][] Data => [
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "byte"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "sbyte"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "short"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "ushort"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "int"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "uint"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "long"],
    [new[] { "FirstValue", "SecondValue", "ThirdValue" }, "ulong"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "byte"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "sbyte"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "short"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "ushort"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "int"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "uint"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "long"],
    [new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" }, "ulong"],
    [new[] { "North", "East", "South", "West" }, "byte"],
    [new[] { "North", "East", "South", "West" }, "sbyte"],
    [new[] { "North", "East", "South", "West" }, "short"],
    [new[] { "North", "East", "South", "West" }, "ushort"],
    [new[] { "North", "East", "South", "West" }, "int"],
    [new[] { "North", "East", "South", "West" }, "uint"],
    [new[] { "North", "East", "South", "West" }, "long"],
    [new[] { "North", "East", "South", "West" }, "ulong"]
    ];

    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumOrIntegerForEnumType(String[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants)}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType Prop { get; set; }
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
                            new { type = new[] { "integer" } },
                            new { @enum = constants }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumOrIntegerForMultipleEnumTypeProperties(String[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants)}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType Prop1 { get; set; }
                public EnumerationType Prop2 { get; set; }
                public EnumerationType Prop3 { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop1 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants }
                        }
                    },
                    Prop2 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants }
                        }
                    },
                    Prop3 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer" } },
                            new { @enum = constants }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumOrIntegerOrNullForNullableEnumType(String[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants)}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType? Prop { get; set; }
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
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
    [Theory]
    [MemberData(nameof(Data))]
    public void Generates_EnumOrIntegerOrNullForMultipleNullableEnumTypeProperties(String[] constants, String backingType)
    {
        TestSchema(
            $$"""
            enum EnumerationType: {{backingType}}
            {
                {{String.Join(",\n\t", constants)}}
            }

            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema
            {
                public EnumerationType? Prop1 { get; set; }
                public EnumerationType? Prop2 { get; set; }
                public EnumerationType? Prop3 { get; set; }
            }
            """, new Dictionary<String, Object>()
            {
                ["$id"] = "Schema",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop1 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants }
                        }
                    },
                    Prop2 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants }
                        }
                    },
                    Prop3 = new
                    {
                        oneOf = new Object[]
                        {
                            new { type = new[] { "integer", "null" } },
                            new { @enum = constants }
                        }
                    }
                },
                ["additionalProperties"] = false
            });
    }
}