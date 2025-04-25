using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;

namespace GuardRail.Hardware.GuardRailCustom.Device.Configuration;

public sealed class BuzzerConfiguration : IBuzzerConfiguration<int>
{
    /// <inheritdoc />
    public int BuzzerAddress { get; set; }
}