using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

public class CoreLightManager<T> : ILightManager where T : CoreLightManager<T>
{
    protected readonly ILightConfiguration LightConfiguration;
    protected readonly ILightHardwareManager LightManager;
    protected readonly ILogger<T> Logger;

    protected CoreLightManager(
        ILightConfiguration lightConfiguration,
        ILightHardwareManager lightManager,
        ILogger<T> logger)
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