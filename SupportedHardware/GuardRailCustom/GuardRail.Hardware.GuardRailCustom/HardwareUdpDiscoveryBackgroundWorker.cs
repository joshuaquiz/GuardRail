using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class HardwareUdpDiscoveryBackgroundWorker(
    NetworkHardwareCache networkHardwareCache,
    ILogger<GuardRailUdpClient> guardRailUdpClientLogger,
    ILogger<HardwareUdpDiscoveryBackgroundWorker> logger)
    : BackgroundService
{
    private const int AnnounceIntervalMs = 5000;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ScanForHardware(
                stoppingToken);
            await Task.Delay(
                AnnounceIntervalMs,
                stoppingToken);
        }
    }

    private async Task ScanForHardware(
        CancellationToken cancellationToken)
    {
        using var udpClient = new UdpClient();
        var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, GuardRailCustomConstants.UdpDiscoveryPort);
        var requestId = Guid.NewGuid();
        var encryptedData = Encryption.Encrypt(
            string.Join(
                GuardRailCustomConstants.UdpSeparator,
                requestId,
                GuardRailCustomConstants.UdpCommandNames.Sync,
                GetLocalIpAddress()),
            typeof(Encryption).Assembly.FullName!)!;
        var buffer = Encoding.UTF8.GetBytes(encryptedData);
        var okayBuffer = "OKAY"u8.ToArray();
        try
        {
            logger.LogInformation($"Sending {encryptedData} to {broadcastEndpoint}");
            await udpClient.SendAsync(
                buffer,
                buffer.Length,
                broadcastEndpoint);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            await foreach (var result in GetUdpResponses(
                               udpClient,
                               cts.Token))
            {
                var encryptedResponseData = Encoding.UTF8.GetString(
                    result.Buffer);
                logger.LogInformation($"Got {encryptedResponseData} from {result.RemoteEndPoint}");
                var receivedString = Encryption.Decrypt(
                    encryptedResponseData,
                    typeof(Encryption).Assembly.FullName!)!;
                var parts = receivedString.Split(GuardRailCustomConstants.UdpSeparator);
                if (parts.Length != 4
                    || !Guid.TryParse(
                        parts[0],
                        out var receivedRequestId)
                    || receivedRequestId != requestId)
                {
                    continue;
                }

                var hostIp = parts[1];
                var port = parts[2];
                var name = parts[3];
                await udpClient.SendAsync(
                    okayBuffer,
                    okayBuffer.Length,
                    result.RemoteEndPoint);
                networkHardwareCache
                    .AddOrUpdate(
                        name,
                        new CustomHardwareSettings(
                            name,
                            new GuardRailUdpClient(
                                new IPEndPoint(
                                    IPAddress.Parse(
                                        hostIp),
                                    int.Parse(
                                        port)),
                                guardRailUdpClientLogger)));
            }
        }
        catch
        {
            // Ignored.
        }
    }

    private static async IAsyncEnumerable<UdpReceiveResult> GetUdpResponses(
        UdpClient udpClient,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        UdpReceiveResult result;
        while (!cancellationToken.IsCancellationRequested
               && (result = await udpClient.ReceiveAsync(cancellationToken)) != default)
        {
            yield return result;
        }
    }

    private static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }
}