namespace GuardRail.DeviceLogic.Interfaces.Input.Nfc;

/// <summary>
/// Base configuration for NFC input.
/// </summary>
public interface INfcConfiguration
{
    public string SerialPort { get; set; }
}