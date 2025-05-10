using Microsoft.Extensions.Options;

using RhoMicro.CodeAnalysis.WorkerService1;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

_ = builder.Services
    .AddFooOptions(c => c.UseMonitorOptions())
    .AddHostedService<Worker>();

var host = builder.Build();
host.Run();

namespace RhoMicro.CodeAnalysis.WorkerService1
{
    public class Worker(ILogger<Worker> logger, IFooOptions options) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            while(!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{Data}", JsonSerializer.Serialize(
                    new
                    {
                        Type = options.GetType().Name,
                        options.Property,
                        options.IntProperty,
                        BarOptions = new
                        {
                            options.BarOptions.Property
                        },
                        options.IgnoredProperty
                    }, jsonOptions));
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
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

    partial class FooOptionsRegistrationStrategy
    {
        partial class Pattern
        {
            static partial void ConfigureOptionsBuilder(OptionsBuilder<MutableFooOptions> builder, FooOptionsConfiguration config)
                => builder.ValidateDataAnnotations().ValidateOnStart();
        }
    }

    // options interfaces must be partial to allow for a generated static default instance property
    [Options]
    public partial interface IFooOptions
    {
        // properties must be of type primitive or an [Options] annotated interface
        // or provide a default implementation

        // set/init/readonly is implemented as per the interface

        // This property is included as it has no default implementation. The
        // default value may be specified and will be used in the generated record.
        // The location of the expression is emitted into a #line directive in the
        // record, so we get appropriate intellisense and errors for erroneous
        // expressions in user code.
        [DefaultValueExpression(@"""Default Value""")]
        // Any other attributes will be included in all generated classes.
        [Required(AllowEmptyStrings = false)]
        String Property { get; }
        [AllowedValues(42, 13L)]
        Int32 IntProperty { get; }

        IBarOptions BarOptions { get; }

        [Unbound]
        String IgnoredProperty => Property.ToLowerInvariant();
    }
    [Options]
    public partial interface IBarOptions
    {
        Int32 Property { get; }
    }
}