using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class UdpListenerBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    IServiceProvider serviceProvider,
    ILogger<GuardRailUdpClient> udpClientLogger,
    ILogger<UdpListenerBackgroundWorker> logger)
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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            (string? Response, IPEndPoint ReceivedFrom)? result;
            while (!stoppingToken.IsCancellationRequested
                   && (result = await localListenerUdpClient.ReceiveEncryptedData(cts.Token)) != default)
            {
                try
                {
                    logger.LogGuardRailInformation($"Got {result.Value.Response} from {result.Value.ReceivedFrom}");
                    var sections = result.Value.Response?.Split(GuardRailCustomConstants.UdpSeparator) ?? [];
                    if (sections.Length != 3)
                    {
                        continue;
                    }

                    logger.LogGuardRailInformation($"Sections {sections[0]} {sections[1]} {sections[2]}");
                    await serviceProvider
                        .GetRequiredKeyedService<IUdpCommandHandler>(
                            sections[1])
                        .HandleCommand(
                            sections[2],
                            cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Ignored.
                }
                catch (Exception e)
                {
                    logger.LogGuardRailError(
                        e,
                        $"Error processing UDP command: {e.Message}");
                }
            }
        }
    }
}