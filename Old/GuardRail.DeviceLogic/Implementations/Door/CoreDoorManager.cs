using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Door;

public abstract class CoreDoorManager<T> : IDoorManager where T : CoreDoorManager<T>
{
    protected readonly IDoorConfiguration DoorConfiguration;
    protected readonly ILockableDoorHardwareManager? LockableDoorHardwareManager;
    protected readonly IOpenableDoorHardwareManager? OpenableDoorHardwareManager;
    protected readonly ILogger<T> Logger;

    protected CoreDoorManager(
        IDoorConfiguration doorConfiguration,
        ILockableDoorHardwareManager? lockableDoorHardwareManager,
        IOpenableDoorHardwareManager? openableDoorHardwareManager,
        ILogger<T> logger)
    {
        DoorConfiguration = doorConfiguration;
        LockableDoorHardwareManager = lockableDoorHardwareManager;
        OpenableDoorHardwareManager = openableDoorHardwareManager;
        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async ValueTask UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual async ValueTask LockAsync(
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual async ValueTask OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual async ValueTask CloseAsync(
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        /*if (LockableDoorHardwareManager is not null)
        {
            await LockableDoorHardwareManager.DisposeAddressAsync(DoorConfiguration.DoorAddress);
        }
        await DoorManager.DisposeAddressAsync(
            DoorConfiguration.DoorAddress);*/
    }
}