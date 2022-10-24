using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyScreenManager : CoreScreenManager<EmptyScreenManager>
{
    public EmptyScreenManager()
        : base(null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask DisplayMessageAsync(
        string message,
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}