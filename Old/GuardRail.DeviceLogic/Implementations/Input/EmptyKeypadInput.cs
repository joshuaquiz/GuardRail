using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Input;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyKeypadInput : CoreKeypadInput<EmptyKeypadInput, IKeypadConfiguration<int>, int>
{
    private readonly ILogger<EmptyKeypadInput> _logger;

    public EmptyKeypadInput(ILogger<EmptyKeypadInput> logger)
        : base(null!, null!, null!, null!)
    {
        _logger = logger;
        _logger.LogGuardRailInformation("Starting");
    }

    /// <inheritdoc />
    public override ValueTask OnKeypadReset(
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation("Keypad resetting");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask OnKeypadSubmit(
        string inputData,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation($"Submitting keypad data: \"{inputData}\"");
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _logger.LogGuardRailInformation("Disposing");
        return ValueTask.CompletedTask;
    }
}