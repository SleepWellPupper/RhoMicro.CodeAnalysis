// SPDX-License-Identifier: MPL-2.0

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

/// <summary>
/// Inspects an assembly for <see cref="GeneratedJsonSchemaAttribute"/>
/// annotations and emits the json schemata provided by them to a directory.
/// </summary>
/// <param name="settings">
/// The object providing settings to the service.
/// </param>
/// <param name="logger">
/// The logger to use when logging progress.
/// </param>
/// <param name="lifetime">
/// The lifetime of the application hosting the service. When done, the service
/// will stop the application via this object.
/// </param>
public sealed class MainService(Settings settings, ILogger logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    private static readonly JsonSerializerOptions _schemaSerializationOptions = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
        PropertyNamingPolicy = null
    };
    /// <summary>
    /// Creates a new instance of the service.
    /// </summary>
    /// <param name="settings">
    /// The object providing settings to the service.
    /// </param>
    /// <param name="loggerFactory">
    /// The factory to use when creating a logger to use when logging progress.
    /// </param>
    /// <param name="lifetime">
    /// The lifetime of the application hosting the service. When done, the
    /// service will stop the application via this object.
    /// </param>
    /// <returns>
    /// A new service instance.
    /// </returns>
    public static MainService Create(Settings settings, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime) =>
        new(settings, loggerFactory.CreateLogger<MainService>(), lifetime);
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        if(!Directory.Exists(settings.SchemataPath))
        {
            _ = Directory.CreateDirectory(settings.SchemataPath);
        }

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
