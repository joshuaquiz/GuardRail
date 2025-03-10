using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// A low-level interface defining calls to a local system.
/// </summary>
public interface ILocationCommunicator
{
    /// <summary>
    /// Lists all non-configured APs for a location.
    /// </summary>
    /// <param name="accessPointType">The type of APs to list.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="string"/> representing the work to get the APs.</returns>
    public Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        AccessPointType accessPointType,
        CancellationToken cancellationToken);
}