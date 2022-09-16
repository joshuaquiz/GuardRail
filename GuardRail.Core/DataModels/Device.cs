using System;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// A device like a phone, NFC tag, etc.
    /// </summary>
    public class Device : IAddableItem
    {
        /// <inheritdoc />
        public Guid Guid { get; set; }

        /// <summary>
        /// The ID of the device.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// A friendly name for the device.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The bytes used to identify a device.
        /// </summary>
        public byte[] ByteId { get; set; }

        /// <summary>
        /// Whether or not the device has been configured.
        /// </summary>
        public bool IsConfigured { get; set; }
    }
}