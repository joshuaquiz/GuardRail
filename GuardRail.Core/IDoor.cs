using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Represents a door locking device.
    /// </summary>
    public interface IDoor
    {
        /// <summary>
        /// A friendlyName for the door.
        /// </summary>
        string FriendlyName { get; set; }

        /// <summary>
        /// Gets the device ID of the hardware.
        /// </summary>
        /// <returns></returns>
        Task<string> GetDeviceId();

        /// <summary>
        /// Gets the current locked status for the door.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="LockedStatus"/></returns>
        Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the door in a locked state.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LockAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the door into an un-locked state.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnLockAsync(CancellationToken cancellationToken);
    }
}