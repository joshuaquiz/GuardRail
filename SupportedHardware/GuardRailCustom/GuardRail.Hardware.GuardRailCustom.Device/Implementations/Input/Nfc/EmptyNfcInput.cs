using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyNfcInput : CoreNfcInput<EmptyNfcInput, INfcConfiguration>
{
    public EmptyNfcInput(ILogger<EmptyNfcInput> logger)
        : base(null!, null!, null!, null!)
    {
        logger.LogGuardRailInformation("Setting up");
    }
}