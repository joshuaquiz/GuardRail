using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.CommandHandlers;

public sealed class SyncUdpCommandHandler(
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILightManager lightManager,
    ILogger<SyncUdpCommandHandler> logger)
    : IUdpCommandHandler
{
    public static string CommandName =>
        GuardRailCustomConstants.UdpCommandNames.Sync;

    public async ValueTask<string?> HandleCommand(
        UdpResponse data,
        CancellationToken cancellationToken)
    {
        if (data.Body.IsNullOrWhiteSpace()
            || !IPAddress.TryParse(
                data.Body,
                out var ipAddress))
        {
            logger.LogGuardRailDebug("The data received did not match the required format");
            await NotifyDisconnected(
                cancellationToken);
            return null;
        }

        await NotifySuccessfullyConnected(
            cancellationToken);
        DeviceConstants.RemoteHostIpAddress = ipAddress;
        guardRailUdpClientFactory.InitializeGuardRailUdpClient(
            ipAddress);
        return $"{GetLocalIpAddress()}{GuardRailCustomConstants.UdpSeparator}{DeviceConstants.DeviceId}";
    }

    private static IPAddress GetLocalIpAddress() =>
        Dns.GetHostEntry(
                Dns.GetHostName())
            .AddressList
            .FirstOrDefault()
        ?? throw new InvalidOperationException("No local IP address found.");

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