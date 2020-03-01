using System;

namespace GuardRail.Definitions
{
    /// <summary>
    /// The data defining what device and 
    /// </summary>
    public sealed class AccessRequestedEventArgs : EventArgs
    {
        private AccessRequestedEventArgs(
            IAccessControlDevice accessControlDevice,
            IDevice device)
        {
            AccessControlDevice = accessControlDevice;
            Device = device;
        }

        /// <summary>
        /// Creates an instance of AccessRequestedEventArgs.
        /// </summary>
        /// <param name="accessControlDevice">The access device the request came from.</param>
        /// <param name="device">The device seeking access.</param>
        /// <returns></returns>
        public static AccessRequestedEventArgs Create(
            IAccessControlDevice accessControlDevice,
            IDevice device) =>
            new AccessRequestedEventArgs(
                accessControlDevice,
                device);

        /// <summary>
        /// The access device the request came from.
        /// </summary>
        public IAccessControlDevice AccessControlDevice { get; }

        /// <summary>
        /// The device seeking access.
        /// </summary>
        public IDevice Device { get; }
    }
}