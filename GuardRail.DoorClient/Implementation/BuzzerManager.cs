using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation;

public sealed class BuzzerManager : IBuzzerManager, IDisposable
{
    private readonly IGpio _gpio;
    private readonly BuzzerConfiguration _buzzerConfiguration;
    private readonly ILogger<BuzzerManager> _logger;

    public BuzzerManager(
        IGpio gpio,
        BuzzerConfiguration buzzerConfiguration,
        ILogger<BuzzerManager> logger)
    {
        _gpio = gpio;
        _buzzerConfiguration = buzzerConfiguration;
        _logger = logger;
        _gpio.OpenPin(_buzzerConfiguration.Pin, PinMode.Output);
    }

    public async Task BuzzAsync(TimeSpan duration, CancellationToken cancellationToken)
    {
        LogDebug("Turning buzzer on");
        _gpio.Write(_buzzerConfiguration.Pin, PinValue.High);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            LogDebug("Turning buzzer off");
            _gpio.Write(_buzzerConfiguration.Pin, PinValue.Low);
        }
    }

    private void LogDebug(string message) =>
        _logger.LogDebug("[light manager] " + message);

    public void Dispose()
    {
        _gpio.ClosePin(_buzzerConfiguration.Pin);
    }
}