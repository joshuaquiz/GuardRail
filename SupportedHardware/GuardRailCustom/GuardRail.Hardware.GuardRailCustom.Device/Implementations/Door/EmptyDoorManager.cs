using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Door;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyDoorManager(ILogger<EmptyDoorManager> logger)
    : CoreDoorManager<EmptyDoorManager, int>(null!, null!, null!)
{
    /// <inheritdoc />
    public override ValueTask UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Unlocking for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask LockAsync(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation("Locking");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Opening for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask CloseAsync(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation("Closing");
        return ValueTask.CompletedTask;
    }
}