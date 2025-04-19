using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device.Services;

public sealed class HardwareUdpDiscoveryBackgroundWorker(
    ILightManager lightManager)
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

                var encryptedData = Encryption.Encrypt(
                    $"{ServiceName}:{GetLocalIpAddress()}:{GetServicePort()}:{DeviceConstants.DeviceId}",
                    typeof(Encryption).Assembly.FullName!)!;
                var buffer = Encoding.UTF8.GetBytes(encryptedData);
                await udpClient.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);
                var okayResult = await udpClient.ReceiveAsync(stoppingToken);
                if (Encoding.UTF8.GetString(
                        okayResult.Buffer) == "OKAY")
                {
                    DeviceConstants.RemoteHostIpAddress = result.RemoteEndPoint.Address;
                    await NotifySuccessfullyConnected(
                        stoppingToken);
                }
                else
                {
                    await NotifyDisconnected(
                        stoppingToken);
                }
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

    private async Task NotifySuccessfullyConnected(
        CancellationToken cancellationToken)
    {
        await lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
    }

    private async Task NotifyDisconnected(
        CancellationToken cancellationToken)
    {
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);
        await lightManager.TurnOnRedLightAsync(TimeSpan.FromMilliseconds(500), cancellationToken);
    }
}