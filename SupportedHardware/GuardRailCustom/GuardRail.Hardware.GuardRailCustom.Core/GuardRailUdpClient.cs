using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed class GuardRailUdpClient : UdpClient
{
    private readonly ConcurrentDictionary<Guid, ObservableCollection<UdpResponse>> _pendingRequests = new();
    private readonly IPEndPoint _localEp;
    private readonly ILogger<GuardRailUdpClient> _logger;

    public GuardRailUdpClient(
        IPEndPoint localEp,
        ILogger<GuardRailUdpClient> logger)
        : base(
            localEp)
    {
        _localEp = localEp;
        _logger = logger;
        this.ConfigureEncryptedTrafficLogging(_logger);
    }

    public event OnUdpRequestReceivedEventHandler? OnUnMatchedRequestReceived;

    /// <summary>
    /// Sends data to the client.
    /// </summary>
    /// <typeparam name="T">The type of the data to send.</typeparam>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="data">The data to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <param name="converter">A converter method (defaults to using ToJson).</param>
    public async Task SendData<T>(
        string commandName,
        T data,
        CancellationToken cancellationToken,
        Func<T, string>? converter = null)
    {
        converter ??= x => x.ToJson();
        await this.SendEncryptedData(
            _localEp,
            $"{Guid.NewGuid()}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{converter(data)}",
            cancellationToken);
    }

    /// <summary>
    /// Receives data asynchronously and returns it as a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the data to receive.</typeparam>
    /// <param name="commandName">The name of the command that is expected.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <param name="converter">A converter method (defaults to using FromJson).</param>
    /// <returns>The received data deserialized to the specified type.</returns>
    public async Task<T?> GetDataAsync<T>(
        string commandName,
        CancellationToken cancellationToken,
        Func<string, T?>? converter = null)
    {
        converter ??= x => x.FromJson<T>();
        var requestId = Guid.NewGuid();
        var incomingData = new ObservableCollection<UdpResponse>();
        var tcs = new TaskCompletionSource<T?>();
        incomingData.CollectionChanged += (_, args) =>
        {
            foreach (string response in args.NewItems ?? new List<string>())
            {
                var convertedValue = converter(
                    response);
                tcs.SetResult(convertedValue);
                _pendingRequests.TryRemove(
                    requestId,
                    out var _);
            }
        };
        cancellationToken
            .Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    _pendingRequests.TryRemove(
                        requestId,
                        out _);
                });
        _pendingRequests.TryAdd(requestId, incomingData);
        await this.SendEncryptedData(
            $"{requestId}:{commandName}",
            cancellationToken);
        return await tcs.Task;
    }

    /// <summary>
    /// Sends a request and receives a response asynchronously, handling out-of-order responses.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data.</typeparam>
    /// <typeparam name="TResponse">The type of the response data.</typeparam>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="requestData">The request data to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <param name="requestConverter">A converter method for the request (defaults to using ToJson).</param>
    /// <param name="responseConverter">A converter method for the response (defaults to using FromJson).</param>
    /// <returns>The response data deserialized to the specified type.</returns>
    public async Task<TResponse?> GetDataAsync<TRequest, TResponse>(
        string commandName,
        TRequest requestData,
        CancellationToken cancellationToken,
        Func<TRequest, string>? requestConverter = null,
        Func<string, TResponse?>? responseConverter = null)
    {
        requestConverter ??= x => x.ToJson();
        responseConverter ??= x => x.FromJson<TResponse>();
        var requestId = Guid.NewGuid();
        var incomingData = new ObservableCollection<UdpResponse>();
        var tcs = new TaskCompletionSource<TResponse?>();
        incomingData.CollectionChanged += (_, args) =>
        {
            foreach (string response in args.NewItems ?? new List<string>())
            {
                var convertedValue = responseConverter(
                    response);
                tcs.SetResult(convertedValue);
                _pendingRequests.TryRemove(
                    requestId,
                    out var _);
            }
        };
        cancellationToken
            .Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    _pendingRequests.TryRemove(
                        requestId,
                        out _);
                });
        _pendingRequests.TryAdd(requestId, incomingData);
        await this.SendEncryptedData(
            $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{requestConverter(requestData)}",
            cancellationToken);
        return await tcs.Task;
    }

    /// <summary>
    /// Sends a request and receives a series of responses asynchronously, handling out-of-order responses.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request data.</typeparam>
    /// <typeparam name="TResponse">The type of the response data.</typeparam>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="requestData">The request data to send.</param>
    /// <param name="handler">A handler for new items.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <param name="requestConverter">A converter method for the request (defaults to using ToJson).</param>
    /// <param name="responseConverter">A converter method for the response (defaults to using FromJson).</param>
    /// <returns>The response data deserialized to the specified type.</returns>
    public Task GetDataSeries<TRequest, TResponse>(
        string commandName,
        TRequest requestData,
        Func<TResponse, Task> handler,
        CancellationToken cancellationToken,
        Func<TRequest, string>? requestConverter = null,
        Func<string, TResponse?>? responseConverter = null)
    {
        requestConverter ??= x => x.ToJson();
        responseConverter ??= x => x.FromJson<TResponse>();
        var requestId = Guid.NewGuid();
        var incomingData = new ObservableCollection<UdpResponse>();
        incomingData.CollectionChanged += async (_, args) =>
        {
            foreach (string response in args.NewItems ?? new List<string>())
            {
                var convertedValue = responseConverter(
                    response);
                if (convertedValue != null)
                {
                    await handler(convertedValue);
                }
            }
        };
        _pendingRequests.TryAdd(requestId, incomingData);
        return Task.Run(
            async () =>
            {
                try
                {
                    await this.SendEncryptedData(
                        $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{requestConverter(requestData)}",
                        cancellationToken);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(
                                TimeSpan.FromSeconds(
                                    1),
                                cancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            // Ignored.
                        }
                    }
                }
                finally
                {
                    _pendingRequests.TryRemove(
                        requestId,
                        out _);
                }
            },
            cancellationToken);
    }

    public async Task StartReceivingData(
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var (receivedString, receivedFrom) = await this.ReceiveEncryptedData(
                    cancellationToken);
                var requestIdEnd = receivedString
                    .IndexOf(
                        GuardRailCustomConstants.UdpSeparator,
                        StringComparison.Ordinal);
                if (requestIdEnd <= -1
                    || !Guid.TryParse(
                        receivedString
                            .AsSpan(
                                0,
                                requestIdEnd)
                            .ToString(),
                        out var requestId))
                {
                    continue;
                }

                var commandNameEnd = receivedString
                    .IndexOf(
                        GuardRailCustomConstants.UdpSeparator,
                        requestIdEnd + 1,
                        StringComparison.Ordinal);
                if (commandNameEnd <= -1)
                {
                    continue;
                }

                var commandName = receivedString
                    .AsSpan(
                        requestIdEnd + 1,
                        commandNameEnd - requestIdEnd - 1)
                    .ToString();
                var udpResponse = new UdpResponse(
                    requestId,
                    commandName,
                    receivedString.Length > commandNameEnd
                        ? receivedString
                            .AsSpan(
                                commandNameEnd + 1)
                            .ToString()
                        : null,
                    receivedFrom);
                if (_pendingRequests
                    .TryGetValue(
                        requestId,
                        out var observableCollection))
                {
                    observableCollection
                        .Add(
                            udpResponse);
                }
                else if (OnUnMatchedRequestReceived != null)
                {
                    _logger.LogGuardRailInformation($"Received new command ({requestId}), processing...");
                    var result = await OnUnMatchedRequestReceived
                        .Invoke(
                            udpResponse,
                            cancellationToken);
                    if (result != null)
                    {
                        _logger.LogGuardRailInformation($"Sending {result} as the response to {requestId}");
                        await this.SendEncryptedData(
                            receivedFrom,
                            $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{result}",
                            cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                _logger
                    .LogGuardRailError(
                        e);
            }
        }
    }
}