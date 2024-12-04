namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Cli;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class MainService(Settings settings, ILogger logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    private static readonly JsonSerializerOptions _schemaSerializationOptions = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
        PropertyNamingPolicy = null
    };

    public static MainService Create(Settings settings, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime) =>
        new(settings, loggerFactory.CreateLogger<MainService>(), lifetime);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var dll = Assembly.LoadFrom(settings.AssemblyPath);
        var schemataAndIdNodes = dll
            .GetCustomAttributes()
            .Where(a => a.GetType().FullName == typeof(GeneratedJsonSchemaAttribute).FullName)
            .Select(a => a
                .GetType()
                .GetProperty(nameof(GeneratedJsonSchemaAttribute.Schema))?
                .GetMethod?
                .Invoke(a, parameters: null) as String)
            .OfType<String>()
            .Select(s => JsonNode.Parse(s) as JsonObject)
            .OfType<JsonObject>()
            .Select(s => (
                idNode: s!.TryGetPropertyValue("$id", out var n) ? n as JsonValue : null,
                schemaNode: s))
            .Where(t => t.idNode is not null)
            .Select(t => (id: t.idNode!.ToString(), t.schemaNode));

        foreach(var (id, schema) in schemataAndIdNodes)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var path = Path.ChangeExtension(Path.Combine(settings.SchemataPath, id), "json");

            logger.LogInformation("Writing schema to '{Path}'.", path);

            if(Path.GetDirectoryName(path) is { } dir)
                _ = Directory.CreateDirectory(dir);

            using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, schema, _schemaSerializationOptions, stoppingToken).ConfigureAwait(false);
        }

        lifetime.StopApplication();
    }
}
