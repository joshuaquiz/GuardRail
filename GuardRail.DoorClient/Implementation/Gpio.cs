using System;
using System.Device.Gpio;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation;

public sealed class Gpio : IGpio, IDisposable
{
    private readonly GpioController _gpio;
    private readonly ILogger<Gpio> _logger;

    public Gpio(GpioController gpio, ILogger<Gpio> logger)
    {
        _gpio = gpio;
        _logger = logger;
        _logger.LogInformation("GPIO wrapper setup");
    }

    /// <inheritdoc />
    public void OpenPin(int pinNumber, PinMode mode) =>
        _gpio.OpenPin(pinNumber, mode);

    /// <inheritdoc />
    public void OpenPin(int pinNumber) =>
        _gpio.OpenPin(pinNumber);

    /// <inheritdoc />
    public void ClosePin(int pinNumber) =>
        _gpio.ClosePin(pinNumber);

    /// <inheritdoc />
    public PinValue Read(int pinNumber) =>
        _gpio.Read(pinNumber);

    /// <inheritdoc />
    public void Write(int pinNumber, PinValue value) =>
        _gpio.Write(pinNumber, value);

    /// <inheritdoc />
    public void SetPinMode(int pinNumber, PinMode mode) =>
        _gpio.SetPinMode(pinNumber, mode);

    /// <inheritdoc />
    public void RegisterCallbackForPinValueChangedEvent(
        int pinNumber,
        PinEventTypes eventTypes,
        PinChangeEventHandler callback) =>
        _gpio.RegisterCallbackForPinValueChangedEvent(
            pinNumber,
            eventTypes,
            callback);

    /// <inheritdoc />
    public void Dispose() =>
        _gpio.Dispose();
}