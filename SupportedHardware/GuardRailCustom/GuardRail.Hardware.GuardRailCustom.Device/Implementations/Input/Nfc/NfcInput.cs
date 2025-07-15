using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public sealed class NfcInput(
    INfcHardwareManager nfcHardwareManager,
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILightManager lightManager,
    ILogger<NfcInput> logger)
    : CoreNfcInput<NfcInput, NfcConfiguration>(
        nfcHardwareManager,
        guardRailUdpClientFactory,
        lightManager,
        logger);