using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Implementations.Input;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using GuardRail.DoorClient.Configuration;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation.Input;

public sealed class KeypadInput : CoreKeypadInput<KeypadInput, KeypadConfiguration, int>
{
    private readonly ILightManager _lightManager;

    public KeypadInput(
        ILightManager lightManager,
        KeypadConfiguration keypadConfiguration,
        ICentralServerCommunication centralServerCommunication,
        IKeypadHardwareManager<int> keypadHardwareManager,
        ILogger<KeypadInput> logger)
        : base(
            keypadConfiguration,
            keypadHardwareManager,
            centralServerCommunication,
            logger)
    {
        _lightManager = lightManager;
    }

    /// <inheritdoc />
    public override async ValueTask OnKeypadReset(CancellationToken cancellationToken)
    {
        await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), cancellationToken);
    }

    public override async ValueTask DisposeAsync()
    {
        foreach (var pin in KeypadConfiguration.ColumnPins.Concat(KeypadConfiguration.RowPins))
        {
            if (KeypadHardwareManager is not null)
            {
                await KeypadHardwareManager.DisposeAddressAsync(pin);
            }
        }
    }
}