using GuardRail.Core.DataModels;

namespace GuardRail.Core;

/// <summary>
/// The data defining the access authorization event
/// </summary>
public sealed class AccessAuthorizationEvent
{
    private AccessAuthorizationEvent(
        Device device,
        IAccessControlDevice accessControlDevice)
    {
        Device = device;
        AccessControlDevice = accessControlDevice;
    }

    /// <summary>
    /// Creates a new AccessAuthorizationEvent.
    /// </summary>
    /// <param name="device">The device that triggered the request.</param>
    /// <param name="accessControlDevice">The access point the request was triggered at.</param>
    /// <returns></returns>
    public static AccessAuthorizationEvent Create(
        Device device,
        IAccessControlDevice accessControlDevice) =>
        new AccessAuthorizationEvent(
            device,
            accessControlDevice);

    /// <summary>
    /// The device that triggered the request.
    /// </summary>
    public Device Device { get; }

    /// <summary>
    /// The access point the request was triggered at.
    /// </summary>
    public IAccessControlDevice AccessControlDevice { get; }
}