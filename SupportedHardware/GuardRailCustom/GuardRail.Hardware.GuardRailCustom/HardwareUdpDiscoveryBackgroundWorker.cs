using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class HardwareUdpDiscoveryBackgroundWorker(
    NetworkHardwareCache networkHardwareCache,
    GuardRailBroadcastUdpClient broadcastUdpClient,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private const int AnnounceIntervalMs = 5000;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var iterationCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                    stoppingToken);
                iterationCancellationToken.CancelAfter(TimeSpan.FromMinutes(1));
                await foreach (var hardwareUdpClientResponse in broadcastUdpClient.GetHardwareOnNetwork(
                                   iterationCancellationToken.Token))
                {
                    networkHardwareCache
                        .AddOrUpdate(
                            hardwareUdpClientResponse.Name,
                            new CustomHardwareSettings(
                                hardwareUdpClientResponse.Name,
                                hardwareUdpClientResponse.Client));
                }
            }
            catch (TaskCanceledException)
            {
                 // Ignored.
            }

            await Task.Delay(
                AnnounceIntervalMs,
                stoppingToken);
        }
    }
}