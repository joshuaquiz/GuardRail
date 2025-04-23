using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Lights;

public sealed class LightHardwareManager(
    IGpio gpio,
    ILightConfiguration<int> lightConfiguration,
    ILogger<LightHardwareManager> logger)
    : ILightHardwareManager<int>
{
    public ValueTask InitAsync()
    {
        logger.LogGuardRailDebug("Starting light hardware, opening pin {lightConfiguration.RedLightAddress} as Output for Red");
        gpio.OpenPin(lightConfiguration.RedLightAddress, PinMode.Output);
        logger.LogGuardRailDebug("Starting light hardware, opening pin {lightConfiguration.GreenLightAddress} as Output for Green");
        gpio.OpenPin(lightConfiguration.GreenLightAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug("Writing High to pin {address}");
        gpio.Write(address, PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug("Writing Low to pin {address}");
        gpio.Write(address, PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        logger.LogGuardRailDebug("Closing pin {address}");
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}