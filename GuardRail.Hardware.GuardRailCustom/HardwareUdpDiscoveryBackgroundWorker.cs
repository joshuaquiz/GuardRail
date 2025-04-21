using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class HardwareUdpDiscoveryBackgroundWorker(
    NetworkHardwareCache networkHardwareCache,
    ILogger<HardwareUdpDiscoveryBackgroundWorker> logger)
    : BackgroundService
{
    private const int DiscoveryPort = 12345;
    private const string ServiceName = "GuardRail";
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
        var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
        var encryptedData = Encryption.Encrypt(
            $"{ServiceName}:{GetLocalIpAddress()}:{GetServicePort()}",
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
                var response = Encryption.Decrypt(
                    encryptedResponseData,
                    typeof(Encryption).Assembly.FullName!)!;
                var parts = response.Split(':');
                if (parts.Length != 4)
                {
                    continue;
                }

                var serviceName = parts.ElementAt(0);
                var hostIp = parts.ElementAt(1);
                var hostPort = parts.ElementAt(2);
                var name = parts.ElementAt(3);
                if (serviceName != ServiceName
                    || !int.TryParse(
                        hostPort,
                        out var port))
                {
                    continue;
                }

                await udpClient.SendAsync(
                    okayBuffer,
                    okayBuffer.Length,
                    result.RemoteEndPoint);
                networkHardwareCache
                    .AddOrUpdate(
                        name,
                        new CustomHardwareSettings(
                            name,
                            new IPEndPoint(
                                IPAddress.Parse(
                                    hostIp),
                                port)));
            }
        }
        catch (Exception e)
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

    private static int GetServicePort()
    {
        return 5001;
    }
}