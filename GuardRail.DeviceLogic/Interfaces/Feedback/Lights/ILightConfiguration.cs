namespace GuardRail.DeviceLogic.Interfaces.Feedback.Lights;

/// <summary>
/// Configuration information for lights.
/// </summary>
/// <typeparam name="T">The type used for the light's addresses.</typeparam>
public interface ILightConfiguration<T>
{
    /// <summary>
    /// The address of the red light.
    /// </summary>
    public T RedLightAddress { get; set; }

    /// <summary>
    /// The address of the green light.
    /// </summary>
    public T GreenLightAddress { get; set; }
}