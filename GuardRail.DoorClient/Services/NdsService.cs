using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Helpers;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services
{
    public sealed class NdsService : IHostedService
    {
        private readonly ILightManager _lightManager;
        private readonly IUdpWrapper _udpWrapper;
        private readonly UdpConfiguration _udpConfiguration;
        private readonly ILogger<LockService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _connected;
        private Task _listener;

        public NdsService(
            ILightManager lightManager,
            IUdpWrapper udpWrapper,
            UdpConfiguration udpConfiguration,
            ILogger<LockService> logger)
        {
            _lightManager = lightManager;
            _udpWrapper = udpWrapper;
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
                                        _logger.LogDebug("Trying to send ping.");
                                        using var udpClient = new UdpClient
                                        {
                                            EnableBroadcast = true
                                        };
                                        var requestData = Encoding.UTF8.GetBytes(ping.ToJson());
                                        await udpClient.SendAsync(
                                            requestData,
                                            requestData.Length,
                                            new IPEndPoint(IPAddress.Broadcast, _udpConfiguration.Port));
                                        serverResponseData = await udpClient.ReceiveAsync();
                                    },
                                    _cancellationTokenSource.Token));
                            _logger.LogDebug("Ping or timer finished.");
                            if (serverResponseData == default)
                            {
                                _logger.LogDebug("Disconnected!");
                                _udpWrapper.SetUdpClient(null);
                                _connected = false;
                                await NotifyDisconnected();
                            }
                            else
                            {
                                var serverResponseString = Encoding.UTF8.GetString(serverResponseData.Buffer);
                                var serverResponse = serverResponseString.FromJson<NdsPing>();
                                if (ping.RequestId == serverResponse?.RequestId && _connected)
                                {
                                    _logger.LogDebug("Still Connected");
                                }
                                else if (ping.RequestId == serverResponse?.RequestId && !_connected)
                                {
                                    _logger.LogDebug("Successfully pinged!");
                                    _logger.LogDebug($"Remote host: {serverResponseData.RemoteEndPoint.Address}:{serverResponseData.RemoteEndPoint.Port}");
                                    try
                                    {
                                        var client = new UdpClient();
                                        client.Connect(serverResponseData.RemoteEndPoint);
                                        _udpWrapper.SetUdpClient(client);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                        throw;
                                    }

                                    _logger.LogDebug("Setup UdpClient!");
                                    _connected = true;
                                    await NotifySuccessfullyConnected();
                                }
                                else
                                {
                                    _logger.LogDebug($"Unknown value... {serverResponseString}");
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
            _listener.Dispose();
            return Task.CompletedTask;
        }

        private async Task NotifySuccessfullyConnected()
        {
            await _lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationTokenSource.Token);
            await _lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
        }

        private async Task NotifyDisconnected()
        {
            await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
            await Task.Delay(TimeSpan.FromMilliseconds(200), _cancellationTokenSource.Token);
            await _lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), _cancellationTokenSource.Token);
        }
    }
}