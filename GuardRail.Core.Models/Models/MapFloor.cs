using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A floor of a map.
/// </summary>
public class MapFloor
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public required Guid Guid { get; set; }

    /// <summary>
    /// The ID of the location map.
    /// </summary>
    public required Guid LocationMapGuid { get; set; }

    /// <summary>
    /// The floor number.
    /// </summary>
    public required int Number { get; set; }
}