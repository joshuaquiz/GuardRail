using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;

namespace GuardRail.Hardware.GuardRailCustom.Device.Feedback.Lights;

public sealed class LightHardwareManager(
    IGpio gpio,
    ILightConfiguration<int> lightConfiguration)
    : ILightHardwareManager<int>
{
    public ValueTask InitAsync()
    {
        gpio.OpenPin(lightConfiguration.RedLightAddress, PinMode.Output);
        gpio.OpenPin(lightConfiguration.GreenLightAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        gpio.Write(address, PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        gpio.Write(address, PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}