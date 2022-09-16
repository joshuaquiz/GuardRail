using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DoorClient.Implementation.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services;

public sealed class ButtonListenerService : IHostedService
{
    private const int TimerIntervalMilliseconds = 20;

    private readonly KeypadHardwareManager _keypadHardwareManager;
    private readonly ILogger<ButtonListenerService> _logger;

    private Timer _dispatcherTimer;

    public ButtonListenerService(
        KeypadHardwareManager keypadHardwareManager,
        ILogger<ButtonListenerService> logger)
    {
        _keypadHardwareManager = keypadHardwareManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GPIO setup for button listener");
        _dispatcherTimer = new Timer(
            _ => _keypadHardwareManager.TimerTick(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(TimerIntervalMilliseconds));
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) =>
        await _dispatcherTimer.DisposeAsync();
}