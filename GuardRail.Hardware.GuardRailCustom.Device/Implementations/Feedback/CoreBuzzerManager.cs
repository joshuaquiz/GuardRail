using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback;

public class CoreBuzzerManager<TCoreBuzzerManager, TBuzzerConfigurationType>
    : IBuzzerManager
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
        Logger.LogGuardRailDebug("Turning buzzer on");
        await BuzzerManager.TurnBuzzerOnAsync(BuzzerConfiguration.BuzzerAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            Logger.LogGuardRailDebug("Turning buzzer off");
            await BuzzerManager.TurnBuzzerOffAsync(BuzzerConfiguration.BuzzerAddress, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        Logger.LogGuardRailDebug("Disposing buzzer manager");
        await BuzzerManager.DisposeAddressAsync(
            BuzzerConfiguration.BuzzerAddress);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask InitAsync()
    {
        Logger.LogGuardRailDebug("Starting buzzer manager");
        await BuzzAsync(
            TimeSpan.FromMilliseconds(200),
            CancellationToken.None);
    }
}