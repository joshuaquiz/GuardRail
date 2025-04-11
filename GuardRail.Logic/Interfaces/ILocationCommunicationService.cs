using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// A high-level interface defining calls to a local system.
/// </summary>
public interface ILocationCommunicationService
{
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