using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public abstract class CoreNfcInput<TNfcInput, TNfcConfiguration>(
    TNfcConfiguration nfcConfiguration,
    INfcHardwareManager nfcHardwareManager,
    GuardRailUdpClientFactory guardRailUdpClientFactory,
    ILogger<TNfcInput> logger)
    : INfcInput
    where TNfcInput : CoreNfcInput<TNfcInput, TNfcConfiguration>
    where TNfcConfiguration : INfcConfiguration
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(
            async () =>
            {
                await foreach (var tag in nfcHardwareManager.ReadTags(_cancellationTokenSource.Token))
                {
                    logger.LogGuardRailDebug($"NFC input submitting {tag}");
                    var sendData = guardRailUdpClientFactory
                        .GetGuardRailUdpClient()
                        ?.SendData(
                            GuardRailCustomConstants.UdpCommandNames.Connect,
                            tag,
                            _cancellationTokenSource.Token);
                    if (sendData != null)
                    {
                        await sendData;
                    }
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