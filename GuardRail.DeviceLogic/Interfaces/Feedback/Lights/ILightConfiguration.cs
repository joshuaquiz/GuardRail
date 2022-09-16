namespace GuardRail.DeviceLogic.Interfaces.Feedback.Lights;

/// <summary>
/// Configuration information for lights.
/// </summary>
public interface ILightConfiguration
{
    /// <summary>
    /// The address of the red light.
    /// </summary>
    public byte[] RedLightAddress { get; set; }

    /// <summary>
    /// The address of the green light.
    /// </summary>
    public byte[] GreenLightAddress { get; set; }
}