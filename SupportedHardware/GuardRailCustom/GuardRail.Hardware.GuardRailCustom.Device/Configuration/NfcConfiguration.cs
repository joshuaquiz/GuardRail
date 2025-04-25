using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;

namespace GuardRail.Hardware.GuardRailCustom.Device.Configuration;

public sealed class NfcConfiguration : INfcConfiguration
{
    /// <inheritdoc />
    public string SerialPort { get; set; } = string.Empty;
}