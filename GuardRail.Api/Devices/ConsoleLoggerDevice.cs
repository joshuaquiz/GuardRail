using System;
using GuardRail.Core;

namespace GuardRail.Api.Devices
{
    /// <summary>
    /// Logs stuff, that's it.
    /// </summary>
    public sealed class ConsoleLoggerDevice : IDevice
    {
        private static Guid ID = Guid.NewGuid();

        /// <summary>
        /// THe ID of the device.
        /// </summary>
        public string Id => ID.ToString();
    }
}