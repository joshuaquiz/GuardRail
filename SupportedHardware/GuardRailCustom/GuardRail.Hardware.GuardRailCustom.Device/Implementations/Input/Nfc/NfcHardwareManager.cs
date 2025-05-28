using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Input.Nfc;
using Iot.Device.Common;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Input.Nfc;

public sealed class NfcHardwareManager(
    INfcConfiguration nfcConfiguration,
    ILoggerFactory loggerFactory,
    ILogger<NfcHardwareManager> logger)
    : INfcHardwareManager
{
    private readonly I2cConnectionSettings _settings = new(nfcConfiguration.BusId, nfcConfiguration.DeviceAddress);

    private Pn532? _pn532;
    private I2cDevice? _device;

    public async ValueTask InitAsync()
    {
        try
        {
            logger.LogGuardRailDebug($"Initializing NFC: b: {_settings.BusId} d: {_settings.DeviceAddress}");
            _device = I2cDevice.Create(_settings);
            _pn532 = new Pn532(_device);
            LogDispatcher.LoggerFactory = loggerFactory;
        }
        catch (Exception e)
        {
            logger.LogGuardRailError(e, "NFC Init Error");
            await DisposeAsync();
        }
    }

    public async IAsyncEnumerable<string> ReadTags([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        logger.LogGuardRailDebug("NFC Task: Starting loop");
        string? lastTarget = null;
        var sameTargetTimes = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var retData = _pn532?.AutoPoll(5, 200, [PollingType.Passive106kbpsISO144443_4A, PollingType.Passive106kbpsISO144443_4B, PollingType.MifareCard]);
            if (retData is null
                || retData.Length < 3)
            {
                continue;
            }

            // Give time to PN532 to process
            await Task.Delay(200, cancellationToken);
            // Check how many tags and the type
            var pollingType = (PollingType)retData[1];
            logger.LogGuardRailDebug($"Num tags: {retData[0]}, Type: {pollingType:G} Length: {retData.Length} Data: : {string.Join(' ', retData.Select(x => x.ToString()))}");
            // See documentation page 145
            // You need to remove the first element at it's the number of tags read
            // In, this case we will assume we are reading only 1 tag at a time
            // The second element is the type of the card.
            // The third element is the size of the data
            string currentTarget;
            switch (pollingType)
            {
                case PollingType.Passive106kbpsISO144443_4A:
                {
                    var decrypted = _pn532?.TryDecode106kbpsTypeA(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted: Type A: {decrypted.ToJson()}");
                    currentTarget = GetDataFromBytes(
                        decrypted?.Ats,
                        decrypted?.NfcId);
                    break;
                }
                case PollingType.Passive106kbpsISO144443_4B:
                {
                    var decrypted = _pn532?.TryDecodeData106kbpsTypeB(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted: Type B {decrypted.ToJson()}");
                    currentTarget = GetDataFromBytes(
                        decrypted?.ApplicationData,
                        decrypted?.NfcId);
                    break;
                }
                case PollingType.MifareCard:
                {
                    var decrypted = _pn532?.TryDecode106kbpsTypeA(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted mifare: {decrypted.ToJson()}");
                    currentTarget = GetDataFromBytes(
                        decrypted?.Ats,
                        decrypted?.NfcId);
                        break;
                }
                default:
                {
                    logger.LogGuardRailDebug($"NFC Task: Unknown type: {pollingType:G}");
                    continue;
                }
            }

            if (!currentTarget.IsNullOrEmpty())
            {
                if (currentTarget == lastTarget)
                {
                    sameTargetTimes++;
                    if (sameTargetTimes > 10)
                    {
                        sameTargetTimes = 0;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    sameTargetTimes = 0;
                    lastTarget = currentTarget;
                }

                yield return currentTarget;
            }
            else
            {
                lastTarget = null;
                sameTargetTimes = 0;
            }
        }
    }

    private static string GetDataFromBytes(
        params byte[]?[] b)
    {
        foreach (var bytes in b)
        {
            if (bytes is { Length: > 0 })
            {
                return bytes.ToJson();
            }
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        logger.LogGuardRailInformation("Disposing NFC manager");
        _pn532?.Dispose();
        return ValueTask.CompletedTask;
    }
}