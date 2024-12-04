using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RhoMicro.CodeAnalysis.JsonSchemaGenerator.Cli;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, l) => l.AddConsole())
    .ConfigureAppConfiguration((_, c) => c.AddCommandLine(args))
    .ConfigureServices((_, s) => s
        .AddHostedService(sp => MainService.Create(
            sp.GetRequiredService<Settings>(),
            sp.GetRequiredService<ILoggerFactory>(),
            sp.GetRequiredService<IHostApplicationLifetime>()))
        .AddSingleton(sp => sp.GetRequiredService<IOptions<Settings>>().Value)
        .AddOptions<Settings>()
        .BindConfiguration("")
        .Validate(
            s => File.Exists(s.AssemblyPath) && Path.GetExtension(s.AssemblyPath) is ".dll",
            $"{nameof(Settings.AssemblyPath)} must point to a dll file.")
        .ValidateOnStart())
    .Build()
    .RunAsync()
    .ConfigureAwait(false);
