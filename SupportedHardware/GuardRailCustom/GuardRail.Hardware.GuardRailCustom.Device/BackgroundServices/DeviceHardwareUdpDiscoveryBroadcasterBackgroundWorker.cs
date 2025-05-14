using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class DeviceHardwareUdpDiscoveryBroadcasterBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    GuardRailUdpClientFactory udpClientFactory,
    ILogger<GuardRailUdpClient> udpClientLogger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var localListenerUdpClient = new UdpClient();
        localListenerUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        var localEndpoint = new IPEndPoint(hardwareDiscoveryPacket.IpAddress, hardwareDiscoveryPacket.Port);
        localListenerUdpClient.Client.Bind(localEndpoint);
        localListenerUdpClient
            .ConfigureEncryptedTrafficLogging(
                udpClientLogger);
        using var broadcastUdpClient = new UdpClient();
        broadcastUdpClient.EnableBroadcast = true;
        broadcastUdpClient
            .ConfigureEncryptedTrafficLogging(
                udpClientLogger);
        var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, GuardRailCustomConstants.UdpDiscoveryPort);
        while (!stoppingToken.IsCancellationRequested)
        {
            await broadcastUdpClient
                .SendEncryptedData(
                    broadcastEndpoint,
                    hardwareDiscoveryPacket.ToJson(),
                    stoppingToken);
            await Task.Delay(
                TimeSpan.FromSeconds(
                    udpClientFactory.GetGuardRailUdpClient() == null
                    ? 5
                    : 30),
                stoppingToken);
        }
    }
}