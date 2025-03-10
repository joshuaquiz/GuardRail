using System;

namespace GuardRail.Api.Models.Requests;

/// <summary>
/// An API request with the data needed to create a new location.
/// </summary>
/// <param name="AccountId">The ID of the account.</param>
/// <param name="IsMobile">Whether this location is mobile.</param>
/// <param name="Name">The name of this location.</param>
/// <param name="Description">A description of this location.</param>
/// <param name="Latitude">The longitude of this location.</param>
/// <param name="Longitude">The longitude of this location.</param>
/// <param name="GeoFenceDistance">The distance from the lat/long allowed to be considered "in-range".</param>
public sealed record CreateNewLocationRequest(
    Guid AccountId,
    bool IsMobile,
    string Name,
    string? Description,
    decimal? Latitude,
    decimal? Longitude,
    decimal? GeoFenceDistance);