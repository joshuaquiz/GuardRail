using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Input;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyNfcInput : CoreNfcInput<EmptyNfcInput, INfcConfiguration>
{
    private readonly ILogger<EmptyNfcInput> _logger;

    public EmptyNfcInput(ILogger<EmptyNfcInput> logger)
        : base(null!, null!, null!, null!)
    {
        _logger = logger;
        _logger.LogGuardRailInformation("Setting up");
    }

    /// <inheritdoc />
    public override ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken)
    {
        _logger.LogGuardRailInformation(nameof(inputData) + ": " + inputData);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        _logger.LogGuardRailInformation("Disposing");
        return ValueTask.CompletedTask;
    }
}