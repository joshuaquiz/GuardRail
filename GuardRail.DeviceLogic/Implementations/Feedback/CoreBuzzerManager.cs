using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

public class CoreBuzzerManager<T> : IBuzzerManager where T : CoreBuzzerManager<T>
{
    protected readonly IBuzzerConfiguration BuzzerConfiguration;
    protected readonly IBuzzerHardwareManager BuzzerManager;
    protected readonly ILogger<T> Logger;

    protected CoreBuzzerManager(
        IBuzzerConfiguration buzzerConfiguration,
        IBuzzerHardwareManager buzzerManager,
        ILogger<T> logger)
    {
        BuzzerConfiguration = buzzerConfiguration;
        BuzzerManager = buzzerManager;
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async ValueTask BuzzAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        LogDebug("Turning buzzer on");
        await BuzzerManager.TurnBuzzerOnAsync(BuzzerConfiguration.BuzzerAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            LogDebug("Turning buzzer off");
            await BuzzerManager.TurnBuzzerOffAsync(BuzzerConfiguration.BuzzerAddress, cancellationToken);
        }
    }

    protected void LogDebug(string message) =>
        Logger.LogDebug("[buzzer manager] " + message);

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        await BuzzerManager.DisposeAddressAsync(
            BuzzerConfiguration.BuzzerAddress);
    }
}