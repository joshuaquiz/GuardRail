using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Manages the link between the device IDs and the hardware behind them.
    /// </summary>
    public interface IDoorResolver
    {
        /// <summary>
        /// Adds a door to the resolver.
        /// </summary>
        /// <param name="door"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RegisterDoor(IDoor door, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an <see cref="IDoor"/> of the hardware for an actual door.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IDoor> GetDoorByDeviceId(string deviceId, CancellationToken cancellationToken);
    }
}