using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Buzzer;

public sealed class BuzzerHardwareManager(
    IGpio gpio,
    IBuzzerConfiguration<int> buzzerConfiguration,
    ILogger<BuzzerHardwareManager> logger)
    : IBuzzerHardwareManager<int>
{
    public ValueTask InitAsync()
    {
        logger.LogGuardRailDebug($"Starting buzzer hardware, opening pin {buzzerConfiguration.BuzzerAddress} as Output");
        gpio.OpenPin(buzzerConfiguration.BuzzerAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug($"Writing High to pin {address}");
        gpio.Write(
            address,
            PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnBuzzerOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug($"Writing Low to pin {buzzerConfiguration.BuzzerAddress}");
        gpio.Write(
            address,
            PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        logger.LogGuardRailDebug($"Closing pin {buzzerConfiguration.BuzzerAddress}");
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}