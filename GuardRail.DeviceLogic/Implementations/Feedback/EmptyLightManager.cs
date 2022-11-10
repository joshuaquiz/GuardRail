using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyLightManager : CoreLightManager<EmptyLightManager, int>
{
    public EmptyLightManager()
        : base(null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask TurnOnRedLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}