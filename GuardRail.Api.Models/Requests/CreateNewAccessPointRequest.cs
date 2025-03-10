using System;
using GuardRail.Core.Models.Enums;

namespace GuardRail.Api.Models.Requests;

/// <summary>
/// An API request with the data needed to create a new access point.
/// </summary>
/// <param name="LocationGuid">The ID of the location.</param>
/// <param name="Name">The AP's friendly name.</param>
/// <param name="AccessPointType">The type of the access point.</param>
/// <param name="Latitude">The latitude of this AP.</param>
/// <param name="Longitude">The longitude of this AP.</param>
/// <param name="GeoFenceDistance">The distance from the lat/long allowed to be considered "in-range".</param>
/// <param name="RequiresAllAccessMethods">Whether all linked access methods are required.</param>
/// <param name="AccessMethodTimeout">The duration for all request/response events to complete before this AP resets.</param>
/// <param name="IsLocked">Whether this AP is locked.</param>
/// <param name="IsOpen">Whether this AP is open.</param>
public sealed record CreateNewAccessPointRequest(
    Guid LocationGuid,
    string Name,
    AccessPointType AccessPointType,
    decimal? Latitude,
    decimal? Longitude,
    decimal? GeoFenceDistance,
    bool RequiresAllAccessMethods,
    TimeSpan? AccessMethodTimeout,
    bool IsLocked,
    bool IsOpen);