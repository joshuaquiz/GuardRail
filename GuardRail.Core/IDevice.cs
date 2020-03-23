using System.Collections.Generic;

namespace GuardRail.Core
{
    /// <summary>
    /// A device like a phone, NFC tag, etc.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// The ID of the device.
        /// </summary>
        string DeviceId { get; }

        /// <summary>
        /// A friendly name for the device.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// The bytes used to identify a device.
        /// </summary>
        public byte[] ByteId { get; }
    }
}