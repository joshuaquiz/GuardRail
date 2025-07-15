using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public abstract class CoreNfcInput<TNfcInput, TNfcConfiguration>(
    INfcHardwareManager nfcHardwareManager,
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILightManager lightManager,
    ILogger<TNfcInput> logger)
    : INfcInput
    where TNfcInput : CoreNfcInput<TNfcInput, TNfcConfiguration>
    where TNfcConfiguration : INfcConfiguration
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(
            async () =>
            {
                await foreach (var tag in nfcHardwareManager.ReadTags(_cancellationTokenSource.Token))
                {
                    logger.LogGuardRailDebug($"NFC input submitting {tag}");
                    var guardRailUdpClient = guardRailUdpClientFactory.GetGuardRailUdpClient();
                    if (guardRailUdpClient == null)
                    {
                        return;
                    }

                    await lightManager.TurnOnGreenLightAsync(TimeSpan.FromMilliseconds(300), _cancellationTokenSource.Token);
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
                    cts.CancelAfter(TimeSpan.FromSeconds(5));
                    await guardRailUdpClient
                        .SendData(
                            GuardRailCustomConstants.UdpCommandNames.UnlockRequest,
                            new UnlockRequestCommandData(
                                DateTimeOffset.UtcNow,
                                TimeSpan.FromSeconds(5),
                                UnlockTriggerType.Nfc,
                                tag,
                                DeviceConstants.DeviceId,
                                DeviceConstants.LocationId!.Value),
                            cts.Token);

                }
            },
            _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationTokenSource.CancelAsync();
        await Task.Delay(500, cancellationToken);
    }
}