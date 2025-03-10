using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A room on a floor of a map.
/// </summary>
public class MapRoom
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
    /// The top left X of the room.
    /// </summary>
    public required long TopLeftX { get; set; }

    /// <summary>
    /// The top left Y of the room.
    /// </summary>
    public required long TopLeftY { get; set; }

    /// <summary>
    /// The top right X of the room.
    /// </summary>
    public required long TopRightX { get; set; }

    /// <summary>
    /// The top right Y of the room.
    /// </summary>
    public required long TopRightY { get; set; }

    /// <summary>
    /// The bottom left X of the room.
    /// </summary>
    public required long BottomLeftX { get; set; }

    /// <summary>
    /// The bottom left Y of the room.
    /// </summary>
    public long BottomLeftY { get; set; }

    /// <summary>
    /// The bottom right X of the room.
    /// </summary>
    public required long BottomRightX { get; set; }

    /// <summary>
    /// The bottom right Y of the room.
    /// </summary>
    public required long BottomRightY { get; set; }
}