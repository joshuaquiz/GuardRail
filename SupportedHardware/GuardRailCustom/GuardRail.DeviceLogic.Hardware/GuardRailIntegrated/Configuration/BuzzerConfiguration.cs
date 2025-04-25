using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;

public sealed class BuzzerConfiguration : IBuzzerConfiguration<int>
{
    /// <inheritdoc />
    public int BuzzerAddress { get; set; }
}