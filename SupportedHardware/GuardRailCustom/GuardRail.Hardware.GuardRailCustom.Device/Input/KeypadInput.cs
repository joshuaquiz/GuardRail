using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Configuration;
using GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Keypad;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Input;

public sealed class KeypadInput(
    ILightManager lightManager,
    KeypadConfiguration keypadConfiguration,
    IKeypadHardwareManager<int> keypadHardwareManager,
    ILogger<KeypadInput> logger)
    : CoreKeypadInput<KeypadInput, KeypadConfiguration, int>(keypadConfiguration,
        keypadHardwareManager,
        logger)
{
    private const int TimerIntervalMilliseconds = 20;

    private readonly Timer? _dispatcherTimer = new(
        _ => keypadHardwareManager.TimerTick(),
        null,
        TimeSpan.Zero,
        TimeSpan.FromMilliseconds(TimerIntervalMilliseconds));

    /// <inheritdoc />
    public override async ValueTask OnKeypadReset(CancellationToken cancellationToken)
    {
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(300), cancellationToken);
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