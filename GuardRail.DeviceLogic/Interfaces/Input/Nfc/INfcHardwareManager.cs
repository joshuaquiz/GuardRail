using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Input.Nfc;

/// <summary>
/// The low level API for interacting with NFC input.
/// </summary>
public interface INfcHardwareManager : IAsyncInit
{
    /// <summary>
    /// The submit event.
    /// </summary>
    public event Func<string, CancellationToken, ValueTask>? Submit;

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        string address);
}