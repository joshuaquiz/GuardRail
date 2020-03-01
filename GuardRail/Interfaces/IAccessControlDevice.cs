using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Interfaces
{
    /// <summary>
    /// Represents an access control device.
    /// This could be a keypad, NFC reader, etc.
    /// </summary>
    public interface IAccessControlDevice
    {
        /// <summary>
        /// Returns the door linked to this device.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="IDoor"/></returns>
        Task<IDoor> GetDoor(CancellationToken cancellationToken);
    }
}