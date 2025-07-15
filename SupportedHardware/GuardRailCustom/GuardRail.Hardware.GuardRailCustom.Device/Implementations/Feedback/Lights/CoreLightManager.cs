using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Lights;

public class CoreLightManager<TCoreLightManager, TLightConfigurationType>
    : ILightManager
    where TCoreLightManager : CoreLightManager<TCoreLightManager, TLightConfigurationType>
{
    protected readonly ILightConfiguration<TLightConfigurationType> LightConfiguration;
    protected readonly ILightHardwareManager<TLightConfigurationType> LightManager;
    protected readonly ILogger<TCoreLightManager> Logger;

    protected CoreLightManager(
        ILightConfiguration<TLightConfigurationType> lightConfiguration,
        ILightHardwareManager<TLightConfigurationType> lightManager,
        ILogger<TCoreLightManager> logger)
    {
        LightConfiguration = lightConfiguration;
        LightManager = lightManager;
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async ValueTask TurnOnRedLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        Logger.LogGuardRailDebug("Turning red light on");
        await LightManager.TurnLightOnAsync(LightConfiguration.RedLightAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            Logger.LogGuardRailDebug("Turning red light off");
            await LightManager.TurnLightOffAsync(LightConfiguration.RedLightAddress, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        Logger.LogGuardRailDebug("Turning green light on");
        await LightManager.TurnLightOnAsync(LightConfiguration.GreenLightAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            Logger.LogGuardRailDebug("Turning green light off");
            await LightManager.TurnLightOffAsync(LightConfiguration.GreenLightAddress, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        Logger.LogGuardRailDebug("Disposing buzzer manager");
        await LightManager.DisposeAddressAsync(
            LightConfiguration.RedLightAddress);
        await LightManager.DisposeAddressAsync(
            LightConfiguration.GreenLightAddress);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask InitAsync()
    {
        Logger.LogGuardRailDebug("Starting light manager");
        await TurnOnRedLightAsync(
            TimeSpan.FromMilliseconds(250),
            CancellationToken.None);
        await TurnOnGreenLightAsync(
            TimeSpan.FromMilliseconds(250),
            CancellationToken.None);
    }
}