using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Responses;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models.Models;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service;

public partial class DevelopmentHttpMessageHandlerOverride(
    ILogger<DevelopmentHttpMessageHandlerOverride> logger)
    : DelegatingHandler
{
    [GeneratedRegex(@"^/Version/VersionCheck\?version=(?:\d+\.?)+$")]
    private static partial Regex VersionCheckRegex();

    [GeneratedRegex(@"^/Command/ListPendingCommands\?locationId=[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?$")]
    private static partial Regex GetPendingCommandsRegex();

    [GeneratedRegex(@"^/Command/UpdateCommand\?commandId=[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?&status=\w+$")]
    private static partial Regex UpdateCommandsRegex();

    private readonly IReadOnlyCollection<MockedHttpRequest> _mockedHttpRequests =
    [
        new(
            VersionCheckRegex(),
            [
                new MockedHttpRequestMethodActions(
                    HttpMethod.Get,
                    (_, _) =>
                        ValueTask.FromResult<HttpContent>(
                            CreateStringContent(
                                new VersionCheckResponse(
                                    true,
                                    null,
                                    null,
                                    null).ToJson())))
            ]),
        new(
            GetPendingCommandsRegex(),
            [
                new MockedHttpRequestMethodActions(
                    HttpMethod.Get,
                    (_, _) =>
                        ValueTask.FromResult<HttpContent>(
                            CreateStringContent(
                                new List<Command>
                                {
                                    new()
                                    {
                                        Guid = Guid.NewGuid(),
                                        LocationGuid = Guid.NewGuid(),
                                        Type = CommandType.Ping,
                                        Status = CommandStatus.Pending,
                                        CreatedDate = DateTimeOffset.UtcNow,
                                        ExpiryDate = DateTimeOffset.UtcNow.AddDays(1),
                                        MaxRetries = 0,
                                        Body = DateTimeOffset.UtcNow.ToString("O").ToJson(),
                                        Attempts = 0
                                    },
                                    new()
                                    {
                                        Guid = Guid.NewGuid(),
                                        LocationGuid = Guid.NewGuid(),
                                        Type = CommandType.GetAvailableAccessPoints,
                                        Status = CommandStatus.Pending,
                                        CreatedDate = DateTimeOffset.UtcNow,
                                        ExpiryDate = DateTimeOffset.UtcNow.AddDays(1),
                                        MaxRetries = 0,
                                        Body = AccessPointType.GuardRailCustom.ToString("G").ToJson(),
                                        Attempts = 0
                                    }
                                }.ToJson())))
            ]),
        new(
            UpdateCommandsRegex(),
            [
                new MockedHttpRequestMethodActions(
                    HttpMethod.Post,
                    (_, _) =>
                        ValueTask.FromResult<HttpContent>(
                            CreateStringContent(
                                string.Empty)))
            ])
    ];

    /// <summary>
    /// Gets a <see cref="ValueTask{T}"/> of <see cref="HttpContent"/> for a given <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to match to.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="ValueTask{T}"/> of <see cref="HttpContent"/>.</returns>
    /// <exception cref="MissingDelegatingHandlerDevelopmentConfiguration">Thrown in a missing request is asked for.</exception>
    private async ValueTask<HttpContent> GetHttpContent(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"{request.Method.Method} {request.RequestUri?.PathAndQuery}");
        if (request.Content != null)
        {
            logger.LogInformation(await request.Content.ReadAsStringAsync(cancellationToken));
        }

        var resultFunction = _mockedHttpRequests
            .FirstOrDefault(x =>
                x.UriPattern.IsMatch(
                    request.RequestUri?.PathAndQuery
                    ?? string.Empty))
            ?.HttpMethodActions
            .FirstOrDefault(x =>
                x.HttpMethod == request.Method)
            ?.GenerateContentFunc(
                request,
                cancellationToken);
        if (!resultFunction.HasValue)
        {
            throw new MissingDelegatingHandlerDevelopmentConfiguration(
                request.RequestUri!.PathAndQuery,
                request.Method);
        }

        return await resultFunction.Value;
    }

    /// <inheritdoc />
    /// <exception cref="MissingDelegatingHandlerDevelopmentConfiguration">Thrown in a missing request is asked for.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) =>
        new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = await GetHttpContent(
                request,
                cancellationToken)
        };

    /// <summary>
    /// Wraps a <see cref="string"/> with a UTF-8 application/json <see cref="StringContent"/>.
    /// </summary>
    /// <param name="content">The string to wrap.</param>
    /// <returns>A UTF-8 application/json <see cref="StringContent"/> of the original <see cref="string"/>.</returns>
    private static StringContent CreateStringContent(
        string content) =>
        new(
            content,
            Encoding.UTF8,
            "application/json");
}