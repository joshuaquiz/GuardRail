using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;

namespace GuardRail.DeviceLogic.Implementations.Input;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyKeypadInput : CoreKeypadInput<EmptyKeypadInput, IKeypadConfiguration>
{
    public EmptyKeypadInput()
        : base(null!, null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask OnKeypadReset(
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask OnKeypadSubmit(
        string inputData,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}