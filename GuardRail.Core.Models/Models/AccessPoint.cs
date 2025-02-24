using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// Represents a unit used to initiate badge, pass-code, swipe, etc. access request.
/// </summary>
public class AccessPoint
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the location.
    /// </summary>
    public required Guid LocationGuid { get; set; }

    /// <summary>
    /// The AP's friendly name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The type of the access point.
    /// </summary>
    public required AccessPointType AccessPointType { get; set; }

    /// <summary>
    /// The latitude of this AP.
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// The longitude of this AP.
    /// </summary>
    public decimal? Longitude { get; set; }

    /// <summary>
    /// The distance from the lat/long allowed to be considered "in-range".
    /// </summary>
    public decimal? GeoFenceDistance { get; set; }

    /// <summary>
    /// Whether all linked access methods are required.
    /// </summary>
    public required bool RequiresAllAccessMethods { get; set; }

    /// <summary>
    /// The duration for all request/response events to complete before this AP resets.
    /// </summary>
    public TimeSpan? AccessMethodTimeout { get; set; }

    /// <summary>
    /// Whether this AP is locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Whether this AP is open.
    /// </summary>
    public bool IsOpen { get; set; }
}