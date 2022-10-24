using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DeviceLogic.Interfaces.Door;

/// <summary>
/// The high level API for interacting with doors.
/// </summary>
public interface IDoorManager : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Unlocks the door.
    /// </summary>
    /// <param name="duration">The duration to leave the door unlocked.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to unlock the door.</returns>
    public ValueTask UnLockAsync(
        TimeSpan duration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Locks the door.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to lock the door.</returns>
    public ValueTask LockAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Opens the door.
    /// </summary>
    /// <param name="duration">The duration to leave the door open.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to open the door.</returns>
    public ValueTask OpenAsync(
        TimeSpan duration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Closes the door.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the work to close the door.</returns>
    public ValueTask CloseAsync(
        CancellationToken cancellationToken);
}