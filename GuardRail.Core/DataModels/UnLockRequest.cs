using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.DataModels;

/// <summary>
/// A request to unlock a door.
/// </summary>
public sealed class UnLockRequest
{
    /// <summary>
    /// The ID of the door.
    /// </summary>
    public Guid DoorId { get; set; }

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