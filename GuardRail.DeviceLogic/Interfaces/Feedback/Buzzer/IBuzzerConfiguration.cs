namespace GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;

/// <summary>
/// Configuration information for buzzer.
/// </summary>
public interface IBuzzerConfiguration
{
    /// <summary>
    /// The address of the buzzer.
    /// </summary>
    public byte[] BuzzerAddress { get; set; }
}