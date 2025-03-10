using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// Represents a unit at a door used to badge, pass-code, swipe, etc.
/// </summary>
public class AccessPoint : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The device's friendly name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The distance allowance of a user accessing the door.
    /// </summary>
    [Required]
    public double DistanceAllowance { get; set; } = double.MaxValue;

    /// <summary>
    /// The Guid of the <see cref="AccessPointGroup"/>, if there is one.
    /// </summary>
    public Guid? AccessPointGroupGuid { get; set; }

    /// <summary>
    /// The <see cref="AccessPointGroup"/>, if there is one.
    /// </summary>
    public AccessPointGroup? AccessPointGroup { get; set; }

    /// <summary>
    /// The <see cref="Doors"/>s, linked to this access point.
    /// </summary>
    public List<Door> Doors { get; set; } = new();
}