namespace GuardRail.Definitions
{
    /// <summary>
    /// A device like a phone, NFC tag, etc.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// The ID of the device.
        /// </summary>
        string Id { get; }
    }
}