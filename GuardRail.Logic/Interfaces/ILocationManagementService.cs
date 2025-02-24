using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for location management.
/// </summary>
public interface ILocationManagementService
{
    /// <summary>
    /// Creates a new location within an account.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="isMobile">Whether this location is mobile.</param>
    /// <param name="name">The name of this location.</param>
    /// <param name="description">A description of this location.</param>
    /// <param name="latitude">The longitude of this location.</param>
    /// <param name="longitude">The longitude of this location.</param>
    /// <param name="geoFenceDistance">The distance from the lat/long allowed to be considered "in-range".</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to create the new location.</returns>
    public Task<Location> CreateNewLocation(
        Guid accountId,
        bool isMobile,
        string name,
        string? description,
        decimal? latitude,
        decimal? longitude,
        decimal? geoFenceDistance,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sets the method of communication used by the system to talk to local systems.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="locationCallbackType">The communication type.</param>
    /// <param name="uri">A potential callback url.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to update the communication method.</returns>
    public Task SetCommunicationMethod(
        Guid locationId,
        LocationCallbackType locationCallbackType,
        Uri? uri,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists all locations for an account.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="Location"/> representing the work to get the locations.</returns>
    public Task<IReadOnlyCollection<Location>> ListLocations(
        Guid accountId,
        CancellationToken cancellationToken);
}