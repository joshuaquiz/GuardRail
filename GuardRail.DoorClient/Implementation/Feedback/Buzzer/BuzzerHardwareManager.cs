using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DoorClient.Interfaces;

namespace GuardRail.DoorClient.Implementation.Feedback.Buzzer;

public sealed class BuzzerHardwareManager : IBuzzerHardwareManager
{
    private readonly IGpio _gpio;
    private readonly IBuzzerConfiguration _buzzerConfiguration;

    public BuzzerHardwareManager(
        IGpio gpio,
        IBuzzerConfiguration buzzerConfiguration)
    {
        _gpio = gpio;
        _buzzerConfiguration = buzzerConfiguration;
    }

    public ValueTask InitAsync()
    {
        _gpio.OpenPin(_buzzerConfiguration.BuzzerAddress[0], PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOnAsync(
        byte[] address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(
            address[0],
            PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOffAsync(
        byte[] address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(
            address[0],
            PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(byte[] address)
    {
        _gpio.ClosePin(address[0]);
        return ValueTask.CompletedTask;
    }
}