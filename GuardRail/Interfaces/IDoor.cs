using System.Threading;
using System.Threading.Tasks;
using GuardRail.Enums;

namespace GuardRail.Interfaces
{
    /// <summary>
    /// Represents a door locking device.
    /// </summary>
    public interface IDoor
    {
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
        Task Lock(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the door into an un-locked state.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UnLock(CancellationToken cancellationToken);
    }
}