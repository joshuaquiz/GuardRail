using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyBuzzerManager : CoreBuzzerManager<EmptyBuzzerManager, int>
{
    public EmptyBuzzerManager()
        : base(null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask BuzzAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}