namespace GuardRail.DeviceLogic.Implementations.Feedback;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyBuzzerManager : CoreBuzzerManager<EmptyBuzzerManager>
{
    public EmptyBuzzerManager()
        : base(null!, null!, null!)
    {
    }

    public override ValueTask BuzzAsync(
        TimeSpan duration,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}