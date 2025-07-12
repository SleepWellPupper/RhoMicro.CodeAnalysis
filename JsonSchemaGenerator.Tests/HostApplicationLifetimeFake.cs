// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Tests;

using Microsoft.Extensions.Hosting;

using System.Threading;

internal sealed class HostApplicationLifetimeFake : IHostApplicationLifetime
{
    public static HostApplicationLifetimeFake Instance { get; } = new();
    public void StopApplication() { }

    public CancellationToken ApplicationStarted { get; }
    public CancellationToken ApplicationStopping { get; }
    public CancellationToken ApplicationStopped { get; }
}
