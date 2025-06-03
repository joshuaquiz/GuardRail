using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class DeviceHardwareUdpDiscoveryBroadcasterBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    GuardRailUdpClientFactory udpClientFactory,
    ILightManager lightManager,
    ILogger<DeviceHardwareUdpDiscoveryBroadcasterBackgroundWorker> logger,
    ILogger<GuardRailUdpClient> udpClientLogger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var successfullyConnected = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken))
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(5));
                    var existingConnection = udpClientFactory.GetGuardRailUdpClient();
                    HardwareDiscoveryPacket? result;
                    if (existingConnection == null)
                    {
                        result = await GetRemoteConnectionDataFromNewConnection(
                            cts.Token);
                    }
                    else
                    {
                        using var guardRailUdpClient = new GuardRailUdpClient(
                            hardwareDiscoveryPacket.EncryptionKey,
                            existingConnection.RemoteEndPoint,
                            udpClientLogger);
                        result = await guardRailUdpClient.GetDataAsync<string, HardwareDiscoveryPacket>(
                            GuardRailCustomConstants.UdpCommandNames.ConfirmConnection,
                            "Confirm",
                            cts.Token);
                    }

                    successfullyConnected = result != null;
                    if (successfullyConnected)
                    {
                        if (existingConnection == null
                            || !existingConnection.RemoteEndPoint.Address.Equals(result!.IpAddress)
                            || existingConnection.RemoteEndPoint.Port != result.Port)
                        {
                            existingConnection?.Dispose();
                            udpClientFactory.InitializeGuardRailUdpClient(
                                result!.EncryptionKey,
                                result.IpAddress,
                                result.Port);
                            await NotifySuccessfullyConnected(
                                    CancellationToken.None)
                                .ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        hardwareDiscoveryPacket.EncryptionKey = Guid.NewGuid().ToString();
                    }
                }

                if (successfullyConnected)
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    await NotifyDisconnected(
                            CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored.
            }
        }
    }

    private async Task<HardwareDiscoveryPacket?> GetRemoteConnectionDataFromNewConnection(
        CancellationToken cancellationToken)
    {
        using var broadcastUdpClient = new UdpClient();
        broadcastUdpClient.EnableBroadcast = true;
        broadcastUdpClient
            .ConfigureEncryptedTrafficLogging(
                udpClientLogger);
        var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, GuardRailCustomConstants.UdpDiscoveryPort);
        broadcastUdpClient
            .ConfigureEncryptedTrafficLogging(
                udpClientLogger);
        await broadcastUdpClient
            .SendEncryptedData(
                broadcastEndpoint,
                hardwareDiscoveryPacket.ToJson(),
                GuardRailCustomConstants.DiscoveryKey,
                cancellationToken);
        using var localListenerUdpClient = new UdpClient();
        localListenerUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        var localEndpoint = new IPEndPoint(hardwareDiscoveryPacket.IpAddress, hardwareDiscoveryPacket.Port);
        localListenerUdpClient.Client.Bind(localEndpoint);
        localListenerUdpClient
            .ConfigureEncryptedTrafficLogging(
                udpClientLogger);
        var resultString = await localListenerUdpClient.ReceiveEncryptedData(
            hardwareDiscoveryPacket.EncryptionKey,
            cancellationToken);
        return resultString?.Response?.FromJson<HardwareDiscoveryPacket>();
    }

    private async Task NotifySuccessfullyConnected(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug("Alerting that the connection attempt was successful");
        await lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
    }

    private async Task NotifyDisconnected(
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug("Alerting that the connection attempt was NON successful");
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
    }
}