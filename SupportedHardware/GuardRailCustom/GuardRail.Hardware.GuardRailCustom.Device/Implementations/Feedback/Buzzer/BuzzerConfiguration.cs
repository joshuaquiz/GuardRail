using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Buzzer;

public sealed class BuzzerConfiguration : IBuzzerConfiguration<int>
{
    /// <inheritdoc />
    public int BuzzerAddress { get; set; }
}