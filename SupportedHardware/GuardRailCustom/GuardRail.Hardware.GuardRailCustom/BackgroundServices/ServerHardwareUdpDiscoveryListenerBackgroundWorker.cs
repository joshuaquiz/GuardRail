using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Core.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.BackgroundServices;

public sealed class ServerHardwareUdpDiscoveryListenerBackgroundWorker(
    HardwareDiscoveryPacket hardwareDiscoveryPacket,
    NetworkHardwareCache networkHardwareCache,
    IServiceProvider serviceProvider)
    : HardwareUdpDiscoveryListenerBackgroundWorkerBase(
        hardwareDiscoveryPacket)
{
    protected override ValueTask HandleNewConnectionDetected(
        HardwareDiscoveryPacket data,
        CancellationToken cancellationToken)
    {
        var guardRailUdpClient = new GuardRailUdpClient(
            new IPEndPoint(data.IpAddress, data.Port),
            serviceProvider.GetRequiredService<ILogger<GuardRailUdpClient>>());
        guardRailUdpClient.OnUnMatchedRequestReceived +=
            async (udpResponse, ct) =>
                await serviceProvider
                    .GetRequiredKeyedService<IUdpCommandHandler>(
                        udpResponse.CommandName)
                    .HandleCommand(
                        udpResponse,
                        ct);
        _ = guardRailUdpClient.StartReceivingData(
            CancellationToken.None);
        networkHardwareCache.AddOrUpdate(
            data.Name,
            new CustomHardwareSettings(
                data.Name,
                guardRailUdpClient));
        return ValueTask.CompletedTask;
    }
}