using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;

namespace GuardRail.DeviceLogic.Implementations.Input;

/// <summary>
/// Provides an empty implementation.
/// </summary>
public sealed class EmptyNfcInput : CoreNfcInput<EmptyNfcInput, INfcConfiguration>
{
    public EmptyNfcInput()
        : base(null!, null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public override ValueTask OnNfcSubmit(
        string inputData,
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}