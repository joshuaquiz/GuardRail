using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A floor of a map.
/// </summary>
public class Floor : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The floor number.
    /// </summary>
    [Required]
    public int Number { get; set; }

    /// <summary>
    /// The <see cref="Room"/>s in this map.
    /// </summary>
    public List<Room> Rooms { get; set; } = new();

    /// <summary>
    /// The <see cref="MapDoor"/>s in this floor.
    /// </summary>
    public MapDoor? MapDoors { get; set; }

    /// <summary>
    /// The <see cref="MapCamera"/>s in this floor.
    /// </summary>
    public MapCamera? MapCameras { get; set; }
}