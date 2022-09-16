using System;

namespace GuardRail.Core.DataModels
{
    /// <summary>
    /// Represents an access control device.
    /// This could be a keypad, NFC reader, etc.
    /// </summary>
    public class AccessPoint : IAddableItem
    {
        /// <inheritdoc />
        public Guid Guid { get; set; }

        /// <summary>
        /// The device's ID.
        /// </summary>
        public string DeviceId { get; set; }
    }
}