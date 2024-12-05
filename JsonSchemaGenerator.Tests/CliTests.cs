#pragma warning disable CA1861 // Avoid constant arrays as arguments
namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging.Abstractions;

using RhoMicro.CodeAnalysis.JsonSchemaGenerator.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class CliTests : TestBase
{
    [Fact]
    public async Task MainService_GeneratesSingleSchema()
    {
        await TestCli(
            """
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
    public async Task MainService_GeneratesMultipleSchemata()
    {
        await TestCli(
            """
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema1
            {
                public object Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema2
            {
                public object Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema1.json",
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
            }, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema2.json",
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
    public async Task MainService_GeneratesRefSchemata()
    {
        await TestCli(
            """
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema1
            {
                public Schema2 Prop { get; set; }
            }
            [RhoMicro.CodeAnalysis.JsonSchema]
            class Schema2
            {
                public object Prop { get; set; }
            }
            """, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema1.json",
                ["type"] = new[] { "object" },
                ["properties"] = new
                {
                    Prop = new Dictionary<String, Object>()
                    {
                        ["$ref"] = $"../{n}/Schema2.json"
                    }
                },
                ["additionalProperties"] = false
            }, n => new Dictionary<String, Object>()
            {
                ["$id"] = $"./{n}/Schema2.json",
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

    private async Task TestCli(String source, params Func<String, Object>[] expectedSchemaFactories)
    {
        var settings = CreateSettings(source, out var assemblyName);
        using(var service = CreateMainService(settings))
        {
            await service.StartAsync(default).ConfigureAwait(ConfigureAwaitOptions.None);
        }

        var schemataPaths = Directory.EnumerateFiles(settings.SchemataPath, "*.json", SearchOption.AllDirectories).ToArray();

        if(schemataPaths.Length != expectedSchemaFactories.Length)
            Assert.Fail($"Expected {expectedSchemaFactories.Length} schemata but found {schemataPaths.Length}.");

        var parsedExpectedSchemata = expectedSchemaFactories
            .Select(f => f.Invoke(assemblyName))
            .Select(s => JsonNode.Parse(JsonSerializer.Serialize(s)) as JsonObject)
            .OfType<JsonObject>()
            .Select(s => (id: s.TryGetPropertyValue("$id", out var id) ? id as JsonValue : null, s))
            .Where(t => t.id is not null)
            .ToDictionary(t => t.id!.ToString(), t => t.s);

        foreach(var schemataPath in schemataPaths)
        {
            using var fs = File.OpenRead(schemataPath);
            var actualSchema = await JsonNode.ParseAsync(fs).ConfigureAwait(ConfigureAwaitOptions.None) as JsonObject;

            Assert.NotNull(actualSchema);
            Assert.True(actualSchema.TryGetPropertyValue("$id", out var i));
            var id = Assert.IsAssignableFrom<JsonValue>(i);
            Assert.True(parsedExpectedSchemata.Remove(id.ToString(), out var expectedSchema), $"Unable to locate expected schema with id '{id}'. It might have already been handled.");

            actualSchema.AssertEquality(expectedSchema);
        }

        if(parsedExpectedSchemata.Count > 0)
            Assert.Fail($"Unhandled expected schemata:\n{String.Join('\n', parsedExpectedSchemata.Values.Select(s => s.ToJsonString()))}");
    }

    private MainService CreateMainService(Settings settings) =>
        new(settings, NullLogger.Instance, HostApplicationLifetimeFake.Instance);

    private Settings CreateSettings(String source, out String assemblyName)
    {
        var dir = Directory.CreateTempSubdirectory(typeof(CliTests).FullName!.Replace('.', '_')).FullName;
        var assemblyPath = Path.Combine(dir, "TestAssembly.dll");
        var schemataPath = Path.Combine(dir, "Schemata");
        var settings = new Settings()
        {
            AssemblyPath = assemblyPath,
            SchemataPath = schemataPath
        };

        using(var peStream = File.Create(assemblyPath))
        {
            Compilation compilation = CreateCompilation(source, out _);
            _ = RunGenerator(ref compilation);
            var emitResult = compilation.Emit(peStream);
            Assert.True(emitResult.Success, "emit error");
            assemblyName = compilation.Assembly.Name;
        }


        return settings;
    }
}
