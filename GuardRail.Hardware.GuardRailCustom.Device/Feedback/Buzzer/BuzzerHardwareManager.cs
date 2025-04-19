using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Buzzer;

public sealed class BuzzerHardwareManager : IBuzzerHardwareManager<int>
{
    private readonly IGpio _gpio;
    private readonly IBuzzerConfiguration<int> _buzzerConfiguration;
    private readonly ILogger<BuzzerHardwareManager> _logger;

    public BuzzerHardwareManager(
        IGpio gpio,
        IBuzzerConfiguration<int> buzzerConfiguration,
        ILogger<BuzzerHardwareManager> logger)
    {
        _gpio = gpio;
        _buzzerConfiguration = buzzerConfiguration;
        _logger = logger;
    }

    public ValueTask InitAsync()
    {
        _gpio.OpenPin(_buzzerConfiguration.BuzzerAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(
            address,
            PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(
            address,
            PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        _gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}