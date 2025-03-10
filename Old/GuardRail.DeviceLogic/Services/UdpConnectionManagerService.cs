using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DeviceLogic.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Services;

public sealed class UdpConnectionManagerService : IHostedService
{
    private readonly ILightManager? _lightManager;
    private readonly ICentralServerPushCommunication _centralServerPushCommunication;
    private readonly IUdpConfiguration _udpConfiguration;
    private readonly ILogger<UdpConnectionManagerService> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private bool _connected;
    private Task? _listener;

    public UdpConnectionManagerService(
        ILightManager? lightManager,
        ICentralServerPushCommunication centralServerPushCommunication,
        IUdpConfiguration udpConfiguration,
        ILogger<UdpConnectionManagerService> logger)
    {
        _lightManager = lightManager;
        _centralServerPushCommunication = centralServerPushCommunication;
        _udpConfiguration = udpConfiguration;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener = new TaskFactory()
            .StartNew(
                async () =>
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var ping = new NdsPing
                        {
                            RequestId = Guid.NewGuid()
                        };
                        UdpReceiveResult serverResponseData = default;
                        await Task.WhenAny(
                            Task.Delay(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token),
                            Task.Run(
                                async () =>
                                {
                                    LogDebug("Trying to send ping.");
                                    using var udpClient = new UdpClient
                                    {
                                        EnableBroadcast = true
                                    };
                                    var requestData = Encoding.UTF8.GetBytes(ping.ToJson());
                                    await udpClient.SendAsync(
                                        requestData,
                                        requestData.Length,
                                        new IPEndPoint(IPAddress.Broadcast, _udpConfiguration.Port));
                                    serverResponseData = await udpClient.ReceiveAsync(cancellationToken);
                                },
                                _cancellationTokenSource.Token));
                        LogDebug("Ping or timer finished.");
                        if (serverResponseData == default)
                        {
                            LogDebug("Disconnected!");
                            _centralServerPushCommunication.SetUdpClient(null);
                            _connected = false;
                            await NotifyDisconnected();
                        }
                        else
                        {
                            var serverResponseString = Encoding.UTF8.GetString(serverResponseData.Buffer);
                            var serverResponse = serverResponseString.FromJson<NdsPing>();
                            if (ping.RequestId == serverResponse?.RequestId && _connected)
                            {
                                LogDebug("Still Connected");
                            }
                            else if (ping.RequestId == serverResponse?.RequestId && !_connected)
                            {
                                LogDebug("Successfully pinged!");
                                LogDebug($"Remote host: {serverResponseData.RemoteEndPoint.Address}:{serverResponseData.RemoteEndPoint.Port}");
                                DeviceConstants.RemoteHostIpAddress = serverResponseData.RemoteEndPoint.Address;
                                try
                                {
                                    var client = new UdpClient();
                                    client.Connect(serverResponseData.RemoteEndPoint);
                                    _centralServerPushCommunication.SetUdpClient(client);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }

                                LogDebug("Setup UdpClient!");
                                _connected = true;
                                await NotifySuccessfullyConnected();
                            }
                            else
                            {
                                LogDebug($"Unknown value... {serverResponseString}");
                            }
                        }

                        await Task.Delay(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token);
                    }
                },
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        _listener?.Dispose();
        return Task.CompletedTask;
    }

    private async Task NotifySuccessfullyConnected()
    {
        if (_lightManager == null)
        {
            return;
        }

        await _lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
        await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationTokenSource.Token);
        await _lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
    }

    private async Task NotifyDisconnected()
    {
        if (_lightManager == null)
        {
            return;
        }
        await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
        await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationTokenSource.Token);
        await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
    }

    private void LogDebug(string message) =>
        _logger.LogDebug("[nds service] " + message);
}