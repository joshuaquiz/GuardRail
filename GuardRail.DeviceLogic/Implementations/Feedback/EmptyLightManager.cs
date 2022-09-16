namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyLightManager : CoreLightManager
{
    public EmptyLightManager()
        : base(null!, null!, null!)
    {
    }

    public override ValueTask TurnOnRedLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask TurnOnGreenLightAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}