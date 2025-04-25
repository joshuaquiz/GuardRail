using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;

public sealed class GuardRailUdpBroadcastCommandListener(
    GuardRailUdpClient udpClient)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await udpClient
                .StartReceivingData(
                    stoppingToken);
        }
    }
}