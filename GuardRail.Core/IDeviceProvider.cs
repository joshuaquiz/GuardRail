using System.Collections.Generic;

namespace GuardRail.Core
{
    /// <summary>
    /// Finds and returns the device (known or un-known from the ID given).
    /// </summary>
    public interface IDeviceProvider
    {
        /// <summary>
        /// Returns a device from the bytes ID.
        /// </summary>
        /// <param name="bytes">The ATS of the connected PICC.</param>
        /// <returns></returns>
        IDevice GetDeviceByByteId(IReadOnlyList<byte> bytes);
    }
}