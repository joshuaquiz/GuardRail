using GuardRail.DeviceLogic.Interfaces.Input.Nfc;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;

public sealed class NfcConfiguration : INfcConfiguration
{
    /// <inheritdoc />
    public string SerialPort { get; set; } = string.Empty;
}