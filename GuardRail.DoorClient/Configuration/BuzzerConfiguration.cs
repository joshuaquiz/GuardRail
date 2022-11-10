using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;

namespace GuardRail.DoorClient.Configuration;

public sealed class BuzzerConfiguration : IBuzzerConfiguration<int>
{
    /// <inheritdoc />
    public int BuzzerAddress { get; set; }
}