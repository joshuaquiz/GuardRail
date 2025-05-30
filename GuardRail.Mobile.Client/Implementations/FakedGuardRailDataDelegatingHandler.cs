using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using G3.Maui.Core.Models;

namespace GuardRail.Mobile.Client.Implementations;

public sealed partial class FakedGuardRailDataDelegatingHandler : BaseDelegatingHandler
{
    [GeneratedRegex("^/api/Account/LogIn$")]
    private static partial Regex AccountLogInRegex();

    [GeneratedRegex("^/api/doors/IsInGeoFence$")]
    private static partial Regex IsInGeoFenceRegex();

    [GeneratedRegex("^/api/users/CreateNewToken$")]
    private static partial Regex CreateNewTokenRegex();

    [GeneratedRegex("^/api/device_actions/UnLockRequest$")]
    private static partial Regex UnLockRequestRegex();

    private static List<MockedHttpRequest> GenerateFakeRequests()
    {
        var serializeUser = JsonSerializer.Serialize(
            FakeData.GetAppUsersProfile());
        return
        [
            new MockedHttpRequest(
                AccountLogInRegex(),
                [
                    new MockedHttpRequestMethodActions(
                        HttpMethod.Post,
                        (_, _) =>
                            ValueTask.FromResult<HttpContent>(
                                CreateStringContent(
                                    serializeUser)))
                ]),
            new MockedHttpRequest(
                IsInGeoFenceRegex(),
                [
                    new MockedHttpRequestMethodActions(
                        HttpMethod.Post,
                        (_, _) =>
                            ValueTask.FromResult<HttpContent>(
                                CreateStringContent(
                                    JsonSerializer.Serialize(
                                        true))))
                ]),
            new MockedHttpRequest(
                CreateNewTokenRegex(),
                [
                    new MockedHttpRequestMethodActions(
                        HttpMethod.Get,
                        (_, _) =>
                            ValueTask.FromResult<HttpContent>(
                                CreateStringContent(
                                    JsonSerializer.Serialize(
                                        Guid.NewGuid().ToString()))))
                ]),
            new MockedHttpRequest(
                UnLockRequestRegex(),
                [
                    new MockedHttpRequestMethodActions(
                        HttpMethod.Post,
                        (_, _) =>
                            ValueTask.FromResult<HttpContent>(
                                CreateStringContent(
                                    JsonSerializer.Serialize(
                                        true))))
                ]),
        ];
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakedGuardRailDataDelegatingHandler"/>.
    /// </summary>
    public FakedGuardRailDataDelegatingHandler()
        : base(
            GenerateFakeRequests())
    {
    }
}