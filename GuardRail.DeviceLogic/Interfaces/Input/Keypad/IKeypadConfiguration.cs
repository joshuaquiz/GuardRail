namespace GuardRail.DeviceLogic.Interfaces.Input.Keypad;

/// <summary>
/// Base configuration for a keypad.
/// </summary>
public interface IKeypadConfiguration
{
    /// <summary>
    /// The length of time to wait until previous input in invalidated.
    /// </summary>
    /// <remarks>
    /// If set to 3 sec. and a user inputs "12" then pauses > 3 sec. then inputs "3" the "12" will no longer be stored.
    /// </remarks>
    public TimeSpan KeypadTimeout { get; set; }

    /// <summary>
    /// The key that will act as the submit key.
    /// </summary>
    /// <remarks>
    /// This key will not record its value but will trigger a submit.
    /// </remarks>
    public char SubmitKey { get; set; }
}