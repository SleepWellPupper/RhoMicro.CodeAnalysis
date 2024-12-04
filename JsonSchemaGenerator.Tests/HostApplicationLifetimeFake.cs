namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;

using Microsoft.Extensions.Hosting;

using System.Threading;

sealed class HostApplicationLifetimeFake : IHostApplicationLifetime
{
    public static HostApplicationLifetimeFake Instance { get; } = new();
    public void StopApplication() { }

    public CancellationToken ApplicationStarted { get; }
    public CancellationToken ApplicationStopping { get; }
    public CancellationToken ApplicationStopped { get; }
}
