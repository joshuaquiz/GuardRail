using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Buzzer;

public sealed class BuzzerHardwareManager(
    IGpio gpio,
    IBuzzerConfiguration<int> buzzerConfiguration,
    ILogger<BuzzerHardwareManager> logger)
    : IBuzzerHardwareManager<int>
{
    private readonly ILogger<BuzzerHardwareManager> _logger = logger;

    public ValueTask InitAsync()
    {
        gpio.OpenPin(buzzerConfiguration.BuzzerAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        gpio.Write(
            address,
            PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        gpio.Write(
            address,
            PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}