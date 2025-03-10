using System;
using GuardRail.Core.Data.Enums;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A request to unlock a door.
/// </summary>
public class UnLockRequest
{
    /// <summary>
    /// The guid of the access point.
    /// </summary>
    public Guid AccessPointGuid { get; set; }

    /// <summary>
    /// The method used to attempt to access the door.
    /// </summary>
    public UnlockRequestType UnlockRequestType { get; set; }

    /// <summary>
    /// The data used.
    /// </summary>
    /// <remarks>
    /// The type of data and how to handle it will be determined by the value of <see cref="UnlockRequestType"/>.
    /// </remarks>
    public byte[] Data { get; set; }
}