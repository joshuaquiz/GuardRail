using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Core.BackgroundServices;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class DeviceHardwareUdpDiscoveryListenerBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    ILightManager lightManager,
    GuardRailUdpClientFactory udpClientFactory,
    ILogger<DeviceHardwareUdpDiscoveryListenerBackgroundWorker> logger)
    : HardwareUdpDiscoveryListenerBackgroundWorkerBase(
        hardwareDiscoveryPacket)
{
    protected override async ValueTask HandleNewConnectionDetected(
        HardwareDiscoveryPacket data,
        CancellationToken cancellationToken)
    {
        var currentClient = udpClientFactory.GetGuardRailUdpClient();
        if (currentClient != null
            && currentClient.LocalEp.Address.Equals(data.IpAddress))
        {
            return;
        }

        udpClientFactory.InitializeGuardRailUdpClient(
            data.IpAddress,
            data.Port);
        await NotifySuccessfullyConnected(
            cancellationToken);
    }

    protected override async ValueTask HandleNoResponse(
        CancellationToken cancellationToken) =>
        await NotifyDisconnected(
            cancellationToken);

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