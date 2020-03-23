using System;
using System.Collections.Generic;
using GuardRail.Core;

namespace GuardRail.Api.Devices
{
    /// <summary>
    /// Logs stuff, that's it.
    /// </summary>
    public sealed class LoggerDevice : IDevice
    {
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// THe ID of the device.
        /// </summary>
        public string DeviceId =>
            _id.ToString();

        /// <summary>
        /// A friendly name for the device.
        /// </summary>
        public string FriendlyName =>
            "default testing device";

        /// <summary>
        /// The bytes used to identify a device.
        /// </summary>
        public byte[] ByteId =>
            new byte[]
            {
                0,
                1,
                2,
                3
            };
    }
}