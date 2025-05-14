using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.CommandHandlers;

public sealed class ConnectUdpCommandHandler(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    GuardRailUdpClientFactory udpClientFactory,
    ILightManager lightManager,
    ILogger<ConnectUdpCommandHandler> logger)
    : IUdpCommandHandler
{
    public static string CommandName =>
        GuardRailCustomConstants.UdpCommandNames.Connect;

    public async ValueTask<string?> HandleCommand(
        string commandBody,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Processing {CommandName}: {commandBody}");
        var data = commandBody.FromJson<HardwareDiscoveryPacket>();
        if (data == null
            || hardwareDiscoveryPacket.IsRunningOnDevice == data.IsRunningOnDevice)
        {
            if (udpClientFactory.GetGuardRailUdpClient() == null)
            {
                await NotifyDisconnected(
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            return null;
        }

        var existingConnection = udpClientFactory.GetGuardRailUdpClient();
        if (existingConnection != null
            && existingConnection.RemoteEndPoint.Address.Equals(data.IpAddress)
            && existingConnection.RemoteEndPoint.Port == data.Port)
        {
            if (udpClientFactory.GetGuardRailUdpClient() == null)
            {
                await NotifyDisconnected(
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            return null;
        }

        udpClientFactory.InitializeGuardRailUdpClient(
            data.IpAddress,
            data.Port);
        await NotifySuccessfullyConnected(
                CancellationToken.None)
            .ConfigureAwait(false);
        return null;
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