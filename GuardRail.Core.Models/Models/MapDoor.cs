using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A door on a map.
/// </summary>
public class MapDoor
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public required Guid Guid { get; set; }

    /// <summary>
    /// The ID of the floor on the map this room is in.
    /// </summary>
    public required Guid MapFloorGuid { get; set; }

    /// <summary>
    /// The top left X of the door.
    /// </summary>
    public required long TopLeftX { get; set; }

    /// <summary>
    /// The top left Y of the door.
    /// </summary>
    public required long TopLeftY { get; set; }

    /// <summary>
    /// The rotation degrees of the door.
    /// </summary>
    public required decimal Rotation { get; set; }

    /// <summary>
    /// The length of the door.
    /// </summary>
    public required int Length { get; set; }
}