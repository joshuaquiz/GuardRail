using System;
using System.Threading.Tasks;

namespace GuardRail.Core;

/// <summary>
/// Represents an access control device.
/// This could be a keypad, NFC reader, etc.
/// </summary>
public interface IAccessControlDevice : IDisposable
{
    /// <summary>
    /// Starts up any processes needed to monitor the device.
    /// </summary>
    /// <returns></returns>
    Task Init();

    /// <summary>
    /// Gets an ID for the device.
    /// </summary>
    /// <returns></returns>
    Task<string> GetDeviceId();

    /// <summary>
    /// Present to the user access was not granted.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    Task PresentNoAccessGranted(string reason);
}