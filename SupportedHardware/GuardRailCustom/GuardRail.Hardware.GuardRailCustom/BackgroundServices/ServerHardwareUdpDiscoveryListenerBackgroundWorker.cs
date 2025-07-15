using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.BackgroundServices;

public sealed class ServerHardwareUdpDiscoveryListenerBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    NetworkHardwareCache networkHardwareCache,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var broadcastResponseUdpClient = new UdpClient();
            broadcastResponseUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            var localEndpoint = new IPEndPoint(IPAddress.Any, GuardRailCustomConstants.UdpDiscoveryPort);
            broadcastResponseUdpClient.Client.Bind(localEndpoint);
            (string? Response, IPEndPoint ReceivedFrom)? result;
            while (!stoppingToken.IsCancellationRequested
                   && (result = await broadcastResponseUdpClient.ReceiveEncryptedData(GuardRailCustomConstants.DiscoveryKey, stoppingToken)) != default)
            {
                try
                {
                    HardwareDiscoveryPacket? data;
                    try
                    {
                        data = result.Value.Response?.FromJson<HardwareDiscoveryPacket>();
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (data == null
                        || hardwareDiscoveryPacket.IsRunningOnDevice == data.IsRunningOnDevice)
                    {
                        continue;
                    }

                    var existingConnection = networkHardwareCache.Get(data.Name);
                    if (existingConnection != null
                        && existingConnection.UdpClient.RemoteEndPoint.Address.Equals(data.IpAddress)
                        && existingConnection.UdpClient.RemoteEndPoint.Port == data.Port)
                    {
                        continue;
                    }

                    existingConnection?.Dispose();
                    var guardRailUdpClient = new GuardRailUdpClient(
                        data.EncryptionKey,
                        new IPEndPoint(hardwareDiscoveryPacket.IpAddress, hardwareDiscoveryPacket.Port),
                        new IPEndPoint(data.IpAddress, data.Port),
                        serviceProvider.GetRequiredService<ILogger<GuardRailUdpClient>>());
                    await guardRailUdpClient.SendRawData(
                        hardwareDiscoveryPacket.ToJson(),
                        stoppingToken);
                    guardRailUdpClient.OnUnMatchedRequestReceived +=
                        async (udpResponse, ct) =>
                            await serviceProvider
                                .GetRequiredKeyedService<IUdpCommandHandler>(
                                    udpResponse.CommandName)
                                .HandleCommand(
                                    udpResponse.Body ?? string.Empty,
                                    ct);
                    _ = guardRailUdpClient.StartReceivingData(
                        CancellationToken.None);
                    networkHardwareCache.AddOrUpdate(
                        data.Name,
                        new CustomHardwareSettings(
                            data.Name,
                            guardRailUdpClient));
                }
                catch (OperationCanceledException)
                {
                    // Ignored.
                }
            }
        }
    }
}