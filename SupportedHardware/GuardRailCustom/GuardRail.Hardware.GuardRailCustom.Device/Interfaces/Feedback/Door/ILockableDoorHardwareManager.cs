using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Door;

/// <summary>
/// The low level API for interacting with a door that unlocks.
/// </summary>
/// <typeparam name="T">The type used for the buzzer's address.</typeparam>
public interface ILockableDoorHardwareManager<in T> : IAsyncInit
{
    /// <summary>
    /// Unlocks the door.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to unlock the door.</returns>
    public ValueTask UnLockAsync(
        T address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Locks the door.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to lock the door.</returns>
    public ValueTask LockAsync(
        T address,
        CancellationToken cancellationToken);

    /// <summary>
    /// Disposes, closes, or resets the hardware.
    /// </summary>
    /// <param name="address">The hardware address.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to dispose/close the hardware.</returns>
    public ValueTask DisposeAddressAsync(
        T address);
}