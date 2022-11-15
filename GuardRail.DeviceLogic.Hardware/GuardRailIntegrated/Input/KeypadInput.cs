using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Configuration;
using GuardRail.DeviceLogic.Implementations.Input;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input;

public sealed class KeypadInput : CoreKeypadInput<KeypadInput, KeypadConfiguration, int>
{
    private const int TimerIntervalMilliseconds = 20;

    private readonly ILightManager _lightManager;
    private readonly Timer? _dispatcherTimer;

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
        _dispatcherTimer = new Timer(
            _ => keypadHardwareManager.TimerTick(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(TimerIntervalMilliseconds));
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
        if (_dispatcherTimer is not null)
        {
            await _dispatcherTimer.DisposeAsync();
        }

        foreach (var pin in KeypadConfiguration.ColumnPins.Concat(KeypadConfiguration.RowPins))
        {
            if (KeypadHardwareManager is not null)
            {
                await KeypadHardwareManager.DisposeAddressAsync(pin);
            }
        }
    }
}