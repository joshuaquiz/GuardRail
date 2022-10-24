using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.DataModels;

/// <summary>
/// Represents a door locking device.
/// </summary>
public class Door : IAddableItem
{
    /// <inheritdoc />
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the door.
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// A friendlyName for the door.
    /// </summary>
    public string FriendlyName { get; set; }

    /// <summary>
    /// The locked status of the door.
    /// </summary>
    public DoorStateRequestType DoorStateRequestType { get; set; }

    /// <summary>
    /// Whether or not the door is fully configured.
    /// </summary>
    public bool IsConfigured { get; set; }
}