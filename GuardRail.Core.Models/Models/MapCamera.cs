using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A camera on a map.
/// </summary>
public class MapCamera
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
    /// The X of the camera.
    /// </summary>
    public required long X { get; set; }

    /// <summary>
    /// The Y of the camera.
    /// </summary>
    public required long Y { get; set; }

    /// <summary>
    /// The rotation degrees of the camera.
    /// </summary>
    public required decimal Rotation { get; set; }

    /// <summary>
    /// The view field length of the camera.
    /// </summary>
    public required int ViewFieldLength { get; set; }

    /// <summary>
    /// The view field degrees of the camera.
    /// </summary>
    public required decimal ViewFieldDegrees { get; set; }
}