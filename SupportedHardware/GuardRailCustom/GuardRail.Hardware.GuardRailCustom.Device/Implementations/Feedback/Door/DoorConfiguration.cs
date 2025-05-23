using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Door;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Door;

public sealed class DoorConfiguration : IDoorConfiguration<int>
{
    /// <inheritdoc />
    public int DoorAddress { get; set; }
}