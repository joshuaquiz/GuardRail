using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class HardwareUdpDiscoveryListenerBackgroundWorker(
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILightManager lightManager,
    ILogger<HardwareUdpDiscoveryListenerBackgroundWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var udpClient = new UdpClient();
            var localEndpoint = new IPEndPoint(IPAddress.Any, GuardRailCustomConstants.UdpDiscoveryPort);
            udpClient.Client.Bind(localEndpoint);
            try
            {
                var result = await udpClient.ReceiveAsync(stoppingToken);
                if (result.RemoteEndPoint.Address
                    .Equals(
                        DeviceConstants.RemoteHostIpAddress))
                {
                    // We do not need to respond in the configuration is still the same.
                    logger.LogGuardRailDebug("The remote hose has not hanged");
                    continue;
                }

                var response = Encryption.Decrypt(
                    Encoding.UTF8.GetString(
                        result.Buffer),
                    typeof(Encryption).Assembly.FullName!)!;
                var parts = response.Split(GuardRailCustomConstants.UdpSeparator);
                if (parts.Length != 3)
                {
                    logger.LogGuardRailDebug("The data received did not match the required format");
                    continue;
                }

                var encryptedData = Encryption.Encrypt(
                    string.Join(
                        GuardRailCustomConstants.UdpSeparator,
                        parts[0],
                        GetLocalIpAddress(),
                        GuardRailCustomConstants.UdpCommandPort,
                        DeviceConstants.DeviceId),
                    typeof(Encryption).Assembly.FullName!)!;
                var buffer = Encoding.UTF8.GetBytes(encryptedData);
                await udpClient.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);
                var okayResult = await udpClient.ReceiveAsync(stoppingToken);
                if (Encoding.UTF8.GetString(
                        okayResult.Buffer) == "OKAY")
                {
                    DeviceConstants.RemoteHostIpAddress = result.RemoteEndPoint.Address;
                    guardRailUdpClientFactory.InitializeGuardRailUdpClient(
                        result.RemoteEndPoint.Address);
                    await NotifySuccessfullyConnected(
                        stoppingToken);
                }
                else
                {
                    await NotifyDisconnected(
                        stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Ignored.
            }
            catch (Exception e)
            {
                logger.LogGuardRailError(e);
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