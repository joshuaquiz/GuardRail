using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public sealed class NfcInput(
    NfcConfiguration nfcConfiguration,
    INfcHardwareManager nfcHardwareManager,
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILogger<NfcInput> logger)
    : CoreNfcInput<NfcInput, NfcConfiguration>(
        nfcConfiguration,
        nfcHardwareManager,
        guardRailUdpClientFactory,
        logger);