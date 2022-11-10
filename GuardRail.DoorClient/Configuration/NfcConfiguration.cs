using GuardRail.DeviceLogic.Interfaces.Input.Nfc;

namespace GuardRail.DoorClient.Configuration;

public sealed class NfcConfiguration : INfcConfiguration
{
    /// <inheritdoc />
    public string SerialPort { get; set; } = string.Empty;
}