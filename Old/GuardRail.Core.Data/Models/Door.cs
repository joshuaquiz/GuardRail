using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// Represents a door locking device.
/// </summary>
public class Door : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The ID of the door.
    /// </summary>
    [Required]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// A friendlyName for the door.
    /// </summary>
    [Required]
    public string FriendlyName { get; set; } = string.Empty;

    /// <summary>
    /// The locked status of the door.
    /// </summary>
    [Required]
    public DoorStateRequestType DoorStateRequestType { get; set; }

    /// <summary>
    /// Whether or not the door is fully configured.
    /// </summary>
    [Required]
    public bool IsConfigured { get; set; } = false;

    /// <summary>
    /// The latitude of the door.
    /// </summary>
    [Required]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude of the door.
    /// </summary>
    [Required]
    public double Longitude { get; set; }

    /// <summary>
    /// The Guid of the <see cref="AccessPoint"/>.
    /// </summary>
    [Required]
    public Guid AccessPointGuid { get; set; }

    /// <summary>
    /// The <see cref="AccessPoint"/>.
    /// </summary>
    public AccessPoint AccessPoint { get; set; } = null!;
}