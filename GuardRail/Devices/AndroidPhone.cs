using GuardRail.Core;

namespace GuardRail.Devices
{
    /// <summary>
    /// Represents an android phone.
    /// </summary>
    public sealed class AndroidPhone : IDevice
    {
        /// <summary>
        /// THe ID of the phone.
        /// </summary>
        public string Id { get; }
    }
}