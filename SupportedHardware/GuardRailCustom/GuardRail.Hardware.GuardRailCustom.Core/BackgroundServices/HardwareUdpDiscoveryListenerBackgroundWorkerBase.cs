using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Core.BackgroundServices;

public abstract class HardwareUdpDiscoveryListenerBackgroundWorkerBase(
    HardwareDiscoveryPacket hardwareDiscoveryPacket)
    : BackgroundService
{
    protected abstract ValueTask HandleNewConnectionDetected(
        HardwareDiscoveryPacket data,
        CancellationToken cancellationToken);

    protected virtual ValueTask HandleNoResponse(
        CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var gotValidResponses = false;
            try
            {
                using (var broadcastUdpClient = new UdpClient())
                {
                    broadcastUdpClient.EnableBroadcast = true;
                    var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, GuardRailCustomConstants.UdpDiscoveryPort);
                    await broadcastUdpClient
                        .SendEncryptedData(
                            broadcastEndpoint,
                            hardwareDiscoveryPacket.ToJson(),
                            stoppingToken);
                }

                using var broadcastResponseUdpClient = new UdpClient();
                broadcastResponseUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                var localEndpoint = new IPEndPoint(IPAddress.Any, GuardRailCustomConstants.UdpDiscoveryPort);
                broadcastResponseUdpClient.Client.Bind(localEndpoint);
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                await foreach (var result in GetUdpResponses(
                                   broadcastResponseUdpClient,
                                   cts.Token))
                {
                    var data = result.Response.FromJson<HardwareDiscoveryPacket>();
                    if (data != null
                        && hardwareDiscoveryPacket.IsRunningOnDevice != data.IsRunningOnDevice)
                    {
                        gotValidResponses = true;
                        await HandleNewConnectionDetected(
                            data,
                            cts.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored.
            }

            if (!gotValidResponses)
            {
                await HandleNoResponse(
                    stoppingToken);
            }
        }
    }

    private static async IAsyncEnumerable<(string Response, IPEndPoint ReceivedFrom)> GetUdpResponses(
        UdpClient udpClient,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        (string Response, IPEndPoint ReceivedFrom) result;
        while (!cancellationToken.IsCancellationRequested
               && (result = await udpClient.ReceiveEncryptedData(cancellationToken)) != default)
        {
            yield return result;
        }
    }
}