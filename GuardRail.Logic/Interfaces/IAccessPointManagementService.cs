using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for accessPoint management.
/// </summary>
public interface IAccessPointManagementService
{
    /// <summary>
    /// Creates a new accessPoint at a location.
    /// </summary>
    /// <param name="locationGuid">The ID of the location.</param>
    /// <param name="name">The AP's friendly name.</param>
    /// <param name="accessPointType">The type of the access point.</param>
    /// <param name="latitude">The latitude of this AP.</param>
    /// <param name="longitude">The longitude of this AP.</param>
    /// <param name="geoFenceDistance">The distance from the lat/long allowed to be considered "in-range".</param>
    /// <param name="requiresAllAccessMethods">Whether all linked access methods are required.</param>
    /// <param name="accessMethodTimeout">The duration for all request/response events to complete before this AP resets.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to create the new accessPoint.</returns>
    public Task CreateNewAccessPoint(
        Guid locationGuid,
        string name,
        AccessPointType accessPointType,
        decimal? latitude,
        decimal? longitude,
        decimal? geoFenceDistance,
        bool requiresAllAccessMethods,
        TimeSpan? accessMethodTimeout,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists all APs configured for a location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="AccessPoint"/> representing the work to get the APs.</returns>
    public Task<IReadOnlyCollection<AccessPoint>> ListAccessPoints(
        Guid locationId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists all non-configured APs for a location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="accessPointType">The type of APs to list.</param>
    /// <param name="timeout">A limit to the duration we wait for the local system to complete the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="string"/> representing the work to get the APs.</returns>
    public Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        Guid locationId,
        AccessPointType accessPointType,
        TimeSpan timeout,
        CancellationToken cancellationToken);
}