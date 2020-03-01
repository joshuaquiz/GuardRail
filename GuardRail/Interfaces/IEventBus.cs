using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Interfaces
{
    /// <summary>
    /// Event handling.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event that a user wants access.
        /// </summary>
        /// <param name="accessControlDevice">The device the request came from.</param>
        /// <param name="device">The device initiating the request.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAccessRequestedEvent(
            IAccessControlDevice accessControlDevice,
            IDevice device,
            CancellationToken cancellationToken);
    }
}