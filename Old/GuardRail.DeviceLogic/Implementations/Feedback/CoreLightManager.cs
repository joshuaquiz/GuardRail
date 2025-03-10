using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

public class CoreLightManager<TCoreLightManager, TLightConfigurationType> : ILightManager
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
        LogDebug("Turning red light on");
        await LightManager.TurnLightOnAsync(LightConfiguration.RedLightAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            LogDebug("Turning red light off");
            await LightManager.TurnLightOffAsync(LightConfiguration.RedLightAddress, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        LogDebug("Turning green light on");
        await LightManager.TurnLightOnAsync(LightConfiguration.GreenLightAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            LogDebug("Turning green light off");
            await LightManager.TurnLightOffAsync(LightConfiguration.GreenLightAddress, cancellationToken);
        }
    }

    protected void LogDebug(string message) =>
        Logger.LogDebug("[light manager] " + message);

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        await LightManager.DisposeAddressAsync(
            LightConfiguration.RedLightAddress);
        await LightManager.DisposeAddressAsync(
            LightConfiguration.GreenLightAddress);
    }
}