using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Logic.Interfaces;

namespace GuardRail.Logic.Implementations.LocationCommunicators;

/// <summary>
/// A communicator used when using the URL callback method to communicate with the local system.
/// </summary>
public sealed class UrlCallbackLocationCommunicator(
    HttpClient httpClient)
    : ILocationCommunicator
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(AccessPointType accessPointType,
        CancellationToken cancellationToken) =>
        await httpClient.GetFromJsonAsync<List<string>>(
            $"/AccessPoint/GetAvailableAccessPoints?{nameof(accessPointType)}={accessPointType}",
            cancellationToken)
        ?? [];
}