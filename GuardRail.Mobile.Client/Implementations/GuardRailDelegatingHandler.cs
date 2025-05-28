using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Mobile.Client.Interfaces;

namespace GuardRail.Mobile.Client.Implementations;

public sealed class GuardRailDelegatingHandler(
    HttpMessageHandler innerHandler,
    IGuardRailStorage storage)
    : DelegatingHandler(
        innerHandler)
{
    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add(
            "X-USER-GUID",
            [
                storage.GetUser()?.UserId.ToString()
            ]);
        return base.SendAsync(
            request,
            cancellationToken);
    }
}