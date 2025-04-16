using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public sealed class HardwareUdpDiscoveryBackgroundWorker
    : BackgroundService
{
    private const int DiscoveryPort = 12345;
    private const string ServiceName = "GuardRail";

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var udpClient = new UdpClient();
            var localEndpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);
            udpClient.Client.Bind(localEndpoint);
            try
            {
                var result = await udpClient.ReceiveAsync(stoppingToken);
                var response = Encryption.Decrypt(
                    Encoding.UTF8.GetString(
                        result.Buffer),
                    typeof(Encryption).Assembly.FullName!)!;
                var parts = response.Split(':');
                if (parts.Length != 4)
                {
                    continue;
                }

                var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
                var encryptedData = Encryption.Encrypt(
                    $"{ServiceName}:{GetLocalIpAddress()}:{GetServicePort()}:{Guid.NewGuid()}",
                    typeof(Encryption).Assembly.FullName!)!;
                var buffer = Encoding.UTF8.GetBytes(encryptedData);
                await udpClient.SendAsync(buffer, buffer.Length, broadcastEndpoint);
            }
            catch
            {
                // Ignored.
            }
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