using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyLightManager : CoreLightManager<EmptyLightManager, int>
{
    private readonly ILogger<EmptyLightManager> _logger;

    public EmptyLightManager(ILogger<EmptyLightManager> logger)
        : base(null!, null!, null!)
    {
        _logger = logger;
        _logger.LogGuardRailInformation("Starting");
    }

    /// <inheritdoc />
    public override ValueTask TurnOnRedLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Turning on red light for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Turning on green light for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _logger.LogGuardRailInformation("Disposing");
        return ValueTask.CompletedTask;
    }
}