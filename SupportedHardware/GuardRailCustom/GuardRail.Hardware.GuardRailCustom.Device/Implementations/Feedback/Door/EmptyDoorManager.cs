using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Door;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyDoorManager(ILogger<EmptyDoorManager> logger)
    : CoreDoorManager<EmptyDoorManager, int>(null!, null!, null!)
{
    /// <inheritdoc />
    public override Task UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Unlocking for {duration:g}");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task LockAsync(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation("Locking");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Opening for {duration:g}");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task CloseAsync(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation("Closing");
        return Task.CompletedTask;
    }
}