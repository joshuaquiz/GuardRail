namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;

/// <summary>
/// Configuration information for the door.
/// </summary>
/// <typeparam name="T">The type used for the door's address.</typeparam>
public interface IDoorConfiguration<T>
{
    /// <summary>
    /// The address of the buzzer.
    /// </summary>
    public T DoorAddress { get; set; }
}