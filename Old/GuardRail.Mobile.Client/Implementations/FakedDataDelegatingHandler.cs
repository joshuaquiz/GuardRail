using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.ApiModels.Responses;

namespace GuardRail.Mobile.Client.Implementations;

public sealed class FakedDataDelegatingHandler(
    HttpMessageHandler innerHandler)
    : DelegatingHandler(
        innerHandler)
{
    private static readonly MobileLoginResponse UserProfile = FakeData.GetAppUsersProfile();
    private static readonly string TrueString = JsonSerializer.Serialize(
        true);

    private readonly Dictionary<string, Dictionary<HttpMethod, Func<HttpRequestMessage, string?>>> _fakeRequests = new()
    {
        {
            "/api/Account/LogIn",
            new Dictionary<HttpMethod, Func<HttpRequestMessage, string?>>
            {
                {
                    HttpMethod.Post,
                    _ =>
                        JsonSerializer.Serialize(
                            UserProfile)
                }
            }
        },
        {
            "/api/doors/IsInGeoFence",
            new Dictionary<HttpMethod, Func<HttpRequestMessage, string?>>
            {
                {
                    HttpMethod.Post,
                    _ => TrueString
                }
            }
        },
        {
            "/api/users/CreateNewToken",
            new Dictionary<HttpMethod, Func<HttpRequestMessage, string?>>
            {
                {
                    HttpMethod.Get,
                    _ =>
                        JsonSerializer.Serialize(
                            Guid.NewGuid().ToString())
                }
            }
        },
        {
            "/api/device_actions/UnLockRequest",
            new Dictionary<HttpMethod, Func<HttpRequestMessage, string?>>
            {
                {
                    HttpMethod.Post,
                    _ => TrueString
                }
            }
        }
    };

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) =>
        Task.FromResult(
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    _fakeRequests
                        .FirstOrDefault(x =>
                            x.Key.StartsWith(
                                request.RequestUri!.AbsolutePath))
                        .Value[request.Method](request)
                    ?? string.Empty,
                    Encoding.UTF8,
                    "application/json")
            });
}