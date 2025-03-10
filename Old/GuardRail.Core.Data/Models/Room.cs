using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A room on a floor of a map.
/// </summary>
public class Room : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The top left X of the room.
    /// </summary>
    [Required]
    public long TopLeftX { get; set; }

    /// <summary>
    /// The top left Y of the room.
    /// </summary>
    [Required]
    public long TopLeftY { get; set; }

    /// <summary>
    /// The top right X of the room.
    /// </summary>
    [Required]
    public long TopRightX { get; set; }

    /// <summary>
    /// The top right Y of the room.
    /// </summary>
    [Required]
    public long TopRightY { get; set; }

    /// <summary>
    /// The bottom left X of the room.
    /// </summary>
    [Required]
    public long BottomLeftX { get; set; }

    /// <summary>
    /// The bottom left Y of the room.
    /// </summary>
    [Required]
    public long BottomLeftY { get; set; }

    /// <summary>
    /// The bottom right X of the room.
    /// </summary>
    [Required]
    public long BottomRightX { get; set; }

    /// <summary>
    /// The bottom right Y of the room.
    /// </summary>
    [Required]
    public long BottomRightY { get; set; }
}