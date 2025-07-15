using System.Device.Gpio;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Interfaces;

/// <summary>
/// GPIO wrapper.
/// </summary>
public interface IGpio
{
    /// <summary>
    /// Opens a pin and sets it to a specific mode.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    /// <param name="mode">The mode to be set.</param>
    void OpenPin(int pinNumber, PinMode mode);

    /// <summary>
    /// Opens a pin in order for it to be ready to use.
    /// The driver attempts to open the pin without changing its mode or value.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    void OpenPin(int pinNumber);

    /// <summary>
    /// Closes an open pin.
    /// If allowed by the driver, the state of the pin is not changed.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    void ClosePin(int pinNumber);

    /// <summary>
    /// Reads the current value of a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    /// <returns>The value of the pin.</returns>
    PinValue Read(int pinNumber);

    /// <summary>
    /// Writes a value to a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    /// <param name="value">The value to be written to the pin.</param>
    void Write(int pinNumber, PinValue value);

    /// <summary>
    /// Sets the mode to a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    /// <param name="mode">The mode to be set.</param>
    void SetPinMode(int pinNumber, PinMode mode);

    /// <summary>
    /// Adds a callback that will be invoked when pinNumber has an event of type eventType.
    /// </summary>
    /// <param name="pinNumber">The pin number in the controller's numbering scheme.</param>
    /// <param name="eventTypes">The event types to wait for.</param>
    /// <param name="callback">The callback method that will be invoked.</param>
    void RegisterCallbackForPinValueChangedEvent(
        int pinNumber,
        PinEventTypes eventTypes,
        PinChangeEventHandler callback);
}