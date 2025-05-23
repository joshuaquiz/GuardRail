using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public sealed class NfcConfiguration : INfcConfiguration
{
    /// <inheritdoc />
    public int BusId { get; set; } = 1;

    /// <inheritdoc />
    public int DeviceAddress { get; set; } = 0x24;
}