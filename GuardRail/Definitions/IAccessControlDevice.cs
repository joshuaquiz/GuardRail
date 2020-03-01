using System;
using System.Threading.Tasks;

namespace GuardRail.Definitions
{
    /// <summary>
    /// Represents an access control device.
    /// This could be a keypad, NFC reader, etc.
    /// </summary>
    public interface IAccessControlDevice : IDisposable
    {
        /// <summary>
        /// This event gets raised when access is requested by a device.
        /// </summary>
        public event AccessRequestedEventHandlerAsync AccessRequestedEvent;

        /// <summary>
        /// Starts up any processes needed to monitor the device.
        /// </summary>
        /// <returns></returns>
        Task Init();

        /// <summary>
        /// Gets an ID for the device.
        /// </summary>
        /// <returns></returns>
        Task<string> GetDeviceId();
    }
}