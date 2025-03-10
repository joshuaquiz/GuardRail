using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Door;

/// <summary>
/// The low level API for interacting with a door that is openable.
/// </summary>
public interface IOpenableDoorHardwareManager : IAsyncInit
{
    /// <summary>
    /// Opens the door.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to open the door.</returns>
    public ValueTask OpenAsync(
        byte[] address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Closes the door.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to close the door.</returns>
    public ValueTask CloseAsync(
        byte[] address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        byte[] address);
}