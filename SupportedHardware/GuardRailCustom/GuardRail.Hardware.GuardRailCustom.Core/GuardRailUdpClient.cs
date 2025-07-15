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

public sealed class GuardRailUdpClient : IDisposable
{
    private readonly ConcurrentDictionary<Guid, ObservableCollection<UdpResponse>> _pendingRequests = new();
    private readonly string _encryptionKey;
    private readonly UdpClient _receivingClient;
    private readonly UdpClient _sendingClient;
    private readonly ILogger<GuardRailUdpClient> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public IPEndPoint LocalEndPoint { get; }

    public IPEndPoint RemoteEndPoint { get; }

    public GuardRailUdpClient(
        string encryptionKey,
        IPEndPoint localEndPoint,
        IPEndPoint remoteEndPoint,
        ILogger<GuardRailUdpClient> logger)
    {
        _encryptionKey = encryptionKey;
        _receivingClient = new UdpClient();
        _sendingClient = new UdpClient();
        _logger = logger;
        LocalEndPoint = localEndPoint;
        RemoteEndPoint = remoteEndPoint;
        _receivingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        _receivingClient.Client.Bind(LocalEndPoint);
        _sendingClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
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
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        converter ??= x => x.ToJson();
        await SendRawData(
            $"{Guid.NewGuid()}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{converter(data)}",
            cts.Token);
    }

    /// <summary>
    /// Sends data to the client.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    public async Task SendRawData(
        string data,
        CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        await _sendingClient.SendEncryptedData(
            RemoteEndPoint,
            data,
            _encryptionKey,
            cts.Token);
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
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
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
        cts.Token
            .Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    _pendingRequests.TryRemove(
                        requestId,
                        out _);
                });
        _pendingRequests.TryAdd(requestId, incomingData);
        await _sendingClient.SendEncryptedData(
            RemoteEndPoint,
            $"{requestId}:{commandName}",
            _encryptionKey,
            cts.Token);
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
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
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
        cts.Token
            .Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    _pendingRequests.TryRemove(
                        requestId,
                        out _);
                });
        _pendingRequests.TryAdd(requestId, incomingData);
        await _sendingClient.SendEncryptedData(
            RemoteEndPoint,
            $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{requestConverter(requestData)}",
            _encryptionKey,
            cts.Token);
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
    public async Task GetDataSeries<TRequest, TResponse>(
        string commandName,
        TRequest requestData,
        Func<TResponse, Task> handler,
        CancellationToken cancellationToken,
        Func<TRequest, string>? requestConverter = null,
        Func<string, TResponse?>? responseConverter = null)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
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
        try
        {
            await _sendingClient.SendEncryptedData(
                RemoteEndPoint,
                $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{requestConverter(requestData)}",
                _encryptionKey,
                cts.Token);
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(
                            1),
                        cts.Token);
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
    }

    public async Task StartReceivingData(
        CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        (string? Response, IPEndPoint ReceivedFrom)? result;
        while (!cts.Token.IsCancellationRequested
               && (result = await _receivingClient.ReceiveEncryptedData(_encryptionKey, cts.Token)) != default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(result.Value.Response))
                {
                    continue;
                }

                var requestIdEnd = result.Value.Response
                    .IndexOf(
                        GuardRailCustomConstants.UdpSeparator,
                        StringComparison.Ordinal);
                if (requestIdEnd <= -1
                    || !Guid.TryParse(
                        result.Value.Response
                            .AsSpan(
                                0,
                                requestIdEnd)
                            .ToString(),
                        out var requestId))
                {
                    continue;
                }

                var commandNameEnd = result.Value.Response
                    .IndexOf(
                        GuardRailCustomConstants.UdpSeparator,
                        requestIdEnd + 1,
                        StringComparison.Ordinal);
                if (commandNameEnd <= -1)
                {
                    continue;
                }

                var commandName = result.Value.Response
                    .AsSpan(
                        requestIdEnd + 1,
                        commandNameEnd - requestIdEnd - 1)
                    .ToString();
                var udpResponse = new UdpResponse(
                    requestId,
                    commandName,
                    result.Value.Response.Length > commandNameEnd
                        ? result.Value.Response
                            .AsSpan(
                                commandNameEnd + 1)
                            .ToString()
                        : null,
                    result.Value.ReceivedFrom);
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
                    _logger.LogGuardRailInformation($"{requestId}: Processing...");
                    var processResult = await OnUnMatchedRequestReceived
                        .Invoke(
                            udpResponse,
                            cts.Token);
                    if (processResult != null)
                    {
                        _logger.LogGuardRailInformation($"{requestId}: Sending {processResult} to {result.Value.ReceivedFrom}");
                        await _sendingClient.SendEncryptedData(
                            result.Value.ReceivedFrom,
                            $"{requestId}{GuardRailCustomConstants.UdpSeparator}{commandName}{GuardRailCustomConstants.UdpSeparator}{result}",
                            _encryptionKey,
                            cts.Token);
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

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _pendingRequests.Clear();
        _receivingClient.Dispose();
        _sendingClient.Dispose();
    }
}