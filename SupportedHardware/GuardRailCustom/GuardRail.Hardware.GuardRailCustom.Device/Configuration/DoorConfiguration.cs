using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;

namespace GuardRail.Hardware.GuardRailCustom.Device.Configuration;

public sealed class DoorConfiguration : IDoorConfiguration<int>
{
    /// <inheritdoc />
    public int DoorAddress { get; set; }
}