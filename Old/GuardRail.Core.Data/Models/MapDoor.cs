using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A door on a map.
/// </summary>
public class MapDoor : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The top left X of the door.
    /// </summary>
    [Required]
    public long TopLeftX { get; set; }

    /// <summary>
    /// The top left Y of the door.
    /// </summary>
    [Required]
    public long TopLeftY { get; set; }

    /// <summary>
    /// The rotation degrees of the door.
    /// </summary>
    [Required]
    public decimal Rotation { get; set; }

    /// <summary>
    /// The length of the door.
    /// </summary>
    [Required]
    public int Length { get; set; }
}