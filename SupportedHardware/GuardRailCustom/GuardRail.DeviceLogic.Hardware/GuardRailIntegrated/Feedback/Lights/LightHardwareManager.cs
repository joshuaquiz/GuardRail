using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Feedback.Lights;

public sealed class LightHardwareManager : ILightHardwareManager<int>
{
    private readonly IGpio _gpio;
    private readonly ILightConfiguration<int> _lightConfiguration;

    public LightHardwareManager(
        IGpio gpio,
        ILightConfiguration<int> lightConfiguration)
    {
        _gpio = gpio;
        _lightConfiguration = lightConfiguration;
    }

    public ValueTask InitAsync()
    {
        _gpio.OpenPin(_lightConfiguration.RedLightAddress, PinMode.Output);
        _gpio.OpenPin(_lightConfiguration.GreenLightAddress, PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOnAsync(
        int address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(address, PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOffAsync(
        int address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(address, PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(int address)
    {
        _gpio.ClosePin(address);
        return ValueTask.CompletedTask;
    }
}