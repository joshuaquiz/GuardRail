using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DoorClient.Interfaces;

namespace GuardRail.DoorClient.Implementation.Feedback.Lights;

public sealed class LightHardwareManager : ILightHardwareManager
{
    private readonly IGpio _gpio;
    private readonly ILightConfiguration _lightConfiguration;

    public LightHardwareManager(
        IGpio gpio,
        ILightConfiguration lightConfiguration)
    {
        _gpio = gpio;
        _lightConfiguration = lightConfiguration;
    }

    public ValueTask InitAsync()
    {
        _gpio.OpenPin(_lightConfiguration.RedLightAddress[0], PinMode.Output);
        _gpio.OpenPin(_lightConfiguration.GreenLightAddress[0], PinMode.Output);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOnAsync(
        byte[] address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(address[0], PinValue.High);
        return ValueTask.CompletedTask;
    }

    public ValueTask TurnLightOffAsync(
        byte[] address,
        CancellationToken cancellationToken)
    {
        _gpio.Write(address[0], PinValue.Low);
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAddressAsync(byte[] address)
    {
        _gpio.ClosePin(address[0]);
        return ValueTask.CompletedTask;
    }
}