using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input.Keypad;

/// <summary>
/// High level API for keypad input.
/// </summary>
public interface IKeypadInput : IAsyncInit, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Handler for the reset event triggered from keypad hardware.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to process the reset event.</returns>
    public ValueTask OnKeypadReset(CancellationToken cancellationToken);

    /// <summary>
    /// Handler for the submit event triggered from keypad hardware.
    /// </summary>
    /// <param name="inputData">The text data that was entered.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to process the submit event.</returns>
    public ValueTask OnKeypadSubmit(string inputData, CancellationToken cancellationToken);
}