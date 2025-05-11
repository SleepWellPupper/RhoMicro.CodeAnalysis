using Microsoft.Extensions.Options;

using RhoMicro.CodeAnalysis.WorkerService1;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

_ = builder.Services
    .AddFoo(c => c.UseMonitorOptions())
    .AddHostedService<Worker>();

var host = builder.Build();
host.Run();

namespace RhoMicro.CodeAnalysis.WorkerService1
{
    partial class BarRegistrationStrategy
    {
        partial class Pattern<TAdapter>
        {
            static partial void ConfigureOptionsBuilder(
                OptionsBuilder<MutableBar> builder,
                BarConfiguration configuration)
                => builder.ValidateDataAnnotations().ValidateOnStart();
        }
    }

    public class Worker(ILogger<Worker> logger, IFoo options) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            while(!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{Data}", JsonSerializer.Serialize(options, jsonOptions));
                await Task.Delay(500, stoppingToken).ConfigureAwait(false);
            }
        }
    }

    // Process:
    // An interface is consumed by the library: public interface IFooOptions
    // An implementation is provided for in-process configuration: public sealed class ImmutableFooOptions
    // An implementation is provided for out-of-process configuration: public sealed class FooOptions

    // By default, FooOptions will be registered via the options pattern
    // In addition to the options pattern, we map the IOptions<FooOptions> onto IFooOptions via some form of '.AddTransient<IFooOptions>(sp => sp.GetRequiredService<IOptions<FooOptions>>().Value)'
    // The registration may be overridden by IFooOptions, e.g. via ImmutableFooOptions
    // Since FooOptions is not explicitly depended upon from anywhere, this causes the options pattern to become inert, unless validation is performed at startup.

    // Related: validate required properties automatically

    // The options model needs to contain (we do not support nested interfaces for now):
    // - Properties
    // - name
    // - namespace
    // - normalized name
    // - because we're including arbitrary expressions (attributes & dve), we also need to clone using statements
    // The property model needs to contain:
    // - Type display string
    // - readonly? init? set?
    // - default value expression
    // - list of attribute expressions
    // - location of the name for diagnostics like non-nullable prop being unassigned
    // The default value expression needs to contain:
    // - expression text
    // - location
    [Options]
    public partial interface IFoo
    {
        [DefaultValueExpression("String.Empty")]
        [AllowedValues(123, 456L)]
        String Prop { get; }
        Int32 IntProp { get; }
        IBar Bar { get; }
    }
    [Options]
    public partial interface IBar
    {
        String StringProp { get; }
    }
}