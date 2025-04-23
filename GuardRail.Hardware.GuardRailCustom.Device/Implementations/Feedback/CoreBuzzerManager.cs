using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback;

public class CoreBuzzerManager<TCoreBuzzerManager, TBuzzerConfigurationType>
    : IBuzzerManager, IAsyncInit
    where TCoreBuzzerManager : CoreBuzzerManager<TCoreBuzzerManager, TBuzzerConfigurationType>
{
    protected readonly IBuzzerConfiguration<TBuzzerConfigurationType> BuzzerConfiguration;
    protected readonly IBuzzerHardwareManager<TBuzzerConfigurationType> BuzzerManager;
    protected readonly ILogger<TCoreBuzzerManager> Logger;

    protected CoreBuzzerManager(
        IBuzzerConfiguration<TBuzzerConfigurationType> buzzerConfiguration,
        IBuzzerHardwareManager<TBuzzerConfigurationType> buzzerManager,
        ILogger<TCoreBuzzerManager> logger)
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
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask InitAsync()
    {
        Logger.LogInformation("Should buzz");
        await BuzzAsync(
            TimeSpan.FromMilliseconds(200),
            CancellationToken.None);
    }
}