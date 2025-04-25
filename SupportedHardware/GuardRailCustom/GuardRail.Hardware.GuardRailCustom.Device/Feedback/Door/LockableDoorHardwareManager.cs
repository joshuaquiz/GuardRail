using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Door;

public sealed class LockableDoorHardwareManager(
    IGpio gpio,
    IDoorConfiguration<int> doorConfiguration,
    ILogger<LockableDoorHardwareManager> logger)
    : ILockableDoorHardwareManager<int>
{
    /// <inheritdoc />
    public ValueTask InitAsync()
    {
        logger.LogGuardRailDebug($"Starting lockable door hardware, opening pin {doorConfiguration.DoorAddress} as Output");
        gpio.OpenPin(doorConfiguration.DoorAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask UnLockAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug($"Writing High to pin {doorConfiguration.DoorAddress}");
        gpio.Write(
            address,
            PinValue.High);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask LockAsync(
        int address,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug($"Writing Low to pin {address}");
        gpio.Write(
            address,
            PinValue.Low);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAddressAsync(
        int address)
    {
        logger.LogGuardRailDebug($"Closing pin {doorConfiguration.DoorAddress}");
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}