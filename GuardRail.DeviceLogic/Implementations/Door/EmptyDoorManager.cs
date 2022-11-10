using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Implementations.Door;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyDoorManager : CoreDoorManager<EmptyDoorManager>
{
    public EmptyDoorManager()
        : base(null!, null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask LockAsync(
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask CloseAsync(
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
}