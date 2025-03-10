using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyBuzzerManager : CoreBuzzerManager<EmptyBuzzerManager, int>
{
    private readonly ILogger<EmptyBuzzerManager> _logger;

    public EmptyBuzzerManager(ILogger<EmptyBuzzerManager> logger)
        : base(null!, null!, null!)
    {
        _logger = logger;
        _logger.LogGuardRailInformation("Starting");
    }

    /// <inheritdoc />
    public override ValueTask BuzzAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Buzzing for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _logger.LogGuardRailInformation("Disposing");
        return ValueTask.CompletedTask;
    }
}