using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models;
using GuardRail.Hardware.Common;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// Implementation of <see cref="IAccessRequestProvider"/>.
/// </summary>
public sealed class AccessRequestProvider(
    HttpClient httpClient)
    : IAccessRequestProvider
{
    /// <inheritdoc />
    public async Task TriggerRequestAccess(
        UnlockRequestCommandData unlockRequest,
        CancellationToken cancellationToken) =>
        await httpClient.PostAsync(
            "/AccessPoint/RequestAccess",
            new StringContent(unlockRequest.ToJson()),
            cancellationToken);
}