using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Door;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyDoorManager : CoreDoorManager<EmptyDoorManager>
{
    private readonly ILogger<EmptyDoorManager> _logger;

    public EmptyDoorManager(ILogger<EmptyDoorManager> logger)
        : base(null!, null!, null!, null!)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override ValueTask UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Unlocking for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask LockAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation("Locking");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Opening for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask CloseAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation("Closing");
        return ValueTask.CompletedTask;
    }
}