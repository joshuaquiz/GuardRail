namespace GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;

/// <summary>
/// Configuration information for buzzer.
/// </summary>
/// <typeparam name="T">The type used for the buzzer's address.</typeparam>
public interface IBuzzerConfiguration<T>
{
    /// <summary>
    /// The address of the buzzer.
    /// </summary>
    public T BuzzerAddress { get; set; }
}