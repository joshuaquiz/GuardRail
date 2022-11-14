using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyScreenManager : CoreScreenManager<EmptyScreenManager>
{
    private readonly ILogger<EmptyScreenManager> _logger;

    public EmptyScreenManager(ILogger<EmptyScreenManager> logger)
        : base(null!)
    {
        _logger = logger;
        _logger.LogGuardRailInformation("Starting");
    }

    /// <inheritdoc />
    public override ValueTask DisplayMessageAsync(
        string message,
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Displaying message: \"{message}\" for {duration:g}");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _logger.LogGuardRailInformation("Disposing");
        return ValueTask.CompletedTask;
    }
}