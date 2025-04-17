using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class HardwareUdpDiscoveryBackgroundWorker(
    NetworkHardwareCache networkHardwareCache)
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
            await udpClient.SendAsync(buffer, buffer.Length, broadcastEndpoint);
            var localEndpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            udpClient.Client.Bind(localEndpoint);
            UdpReceiveResult result;
            while ((result = await udpClient.ReceiveAsync(cancellationToken)) != default)
            {
                var response = Encryption.Decrypt(
                    Encoding.UTF8.GetString(
                        result.Buffer),
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

                await udpClient.SendAsync(okayBuffer, okayBuffer.Length, result.RemoteEndPoint);
                networkHardwareCache
                    .Add(
                        name,
                        new CustomHardwareSettings(
                            name,
                            new IPEndPoint(
                                IPAddress.Parse(
                                    hostIp),
                                port)));
            }
        }
        catch
        {
            // Ignored.
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