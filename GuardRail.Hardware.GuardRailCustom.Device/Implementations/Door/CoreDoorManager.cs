using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Door;

public abstract class CoreDoorManager<T>(
    IDoorConfiguration doorConfiguration,
    ILockableDoorHardwareManager? lockableDoorHardwareManager,
    IOpenableDoorHardwareManager? openableDoorHardwareManager,
    ILogger<T> logger)
    : IDoorManager
    where T : CoreDoorManager<T>
{
    protected readonly IDoorConfiguration DoorConfiguration = doorConfiguration;
    protected readonly ILockableDoorHardwareManager? LockableDoorHardwareManager = lockableDoorHardwareManager;
    protected readonly IOpenableDoorHardwareManager? OpenableDoorHardwareManager = openableDoorHardwareManager;
    protected readonly ILogger<T> Logger = logger;

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