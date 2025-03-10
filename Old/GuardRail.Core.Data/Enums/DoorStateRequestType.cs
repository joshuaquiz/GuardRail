namespace GuardRail.Core.Data.Enums;

/// <summary>
/// The states for a door.
/// </summary>
public enum DoorStateRequestType : byte
{
    /// <summary>
    /// The locked status of the door cannot be determined.
    /// </summary>
    UnKnown = 0,

    /// <summary>
    /// The door is unlocked.
    /// </summary>
    UnLocked = 1,

    /// <summary>
    /// The door is locked.
    /// </summary>
    Locked = 2,

    /// <summary>
    /// The door is open.
    /// </summary>
    Open = 3,

    /// <summary>
    /// The door is closed.
    /// </summary>
    Closed = 4,

    /// <summary>
    /// The green light is on.
    /// </summary>
    GreenLightOn = 5,

    /// <summary>
    /// The red light is on.
    /// </summary>
    RedLightOn = 6,

    /// <summary>
    /// The buzzer is on.
    /// </summary>
    BuzzerOn = 7
}