using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Feedback.Door;

public abstract class CoreDoorManager<TCoreDoorManager, TDoorConfigurationType> : IDoorManager
    where TCoreDoorManager : CoreDoorManager<TCoreDoorManager, TDoorConfigurationType>
{
    protected readonly IDoorConfiguration<TDoorConfigurationType> DoorConfiguration;
    protected readonly ILockableDoorHardwareManager<TDoorConfigurationType> LockableDoorHardwareManager;
    /*protected readonly IOpenableDoorHardwareManager? OpenableDoorHardwareManager;*/
    protected readonly ILogger<TCoreDoorManager> Logger;

    protected CoreDoorManager(IDoorConfiguration<TDoorConfigurationType> doorConfiguration,
        ILockableDoorHardwareManager<TDoorConfigurationType> lockableDoorHardwareManager,
        /*IOpenableDoorHardwareManager? openableDoorHardwareManager,*/
        ILogger<TCoreDoorManager> logger)
    {
        DoorConfiguration = doorConfiguration;
        LockableDoorHardwareManager = lockableDoorHardwareManager;
        /*OpenableDoorHardwareManager = openableDoorHardwareManager;*/
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        Logger.LogGuardRailDebug($"Unlocking door for {duration:g}");
        await LockableDoorHardwareManager.UnLockAsync(DoorConfiguration.DoorAddress, cancellationToken);
        if (duration > TimeSpan.Zero)
        {
            await Task.Delay(duration, cancellationToken);
            Logger.LogGuardRailDebug("Locking door");
            await LockableDoorHardwareManager.LockAsync(DoorConfiguration.DoorAddress, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async Task LockAsync(
        CancellationToken cancellationToken)
    {
        Logger.LogGuardRailDebug("Locking door");
        await LockableDoorHardwareManager.LockAsync(DoorConfiguration.DoorAddress, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual async Task CloseAsync(
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync() =>
        await LockableDoorHardwareManager
            .DisposeAddressAsync(
                DoorConfiguration.DoorAddress);

    public async ValueTask InitAsync()
    {
        Logger.LogGuardRailDebug("Starting door manager");
        await UnLockAsync(
            TimeSpan.FromMilliseconds(500),
            CancellationToken.None);
    }
}