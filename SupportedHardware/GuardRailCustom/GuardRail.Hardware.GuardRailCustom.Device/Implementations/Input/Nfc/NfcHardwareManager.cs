using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        while (!cancellationToken.IsCancellationRequested)
        {
            var retData = _pn532?.AutoPoll(5, 300, [PollingType.Passive106kbpsISO144443_4A, PollingType.Passive106kbpsISO144443_4B, PollingType.MifareCard]);
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
            switch (pollingType)
            {
                case PollingType.Passive106kbpsISO144443_4A:
                {
                    var decrypted = _pn532?.TryDecode106kbpsTypeA(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted: Type A: {decrypted.ToJson()}");
                    yield return Encoding.UTF8.GetString(decrypted?.NfcId ?? []);
                    break;
                }
                case PollingType.Passive106kbpsISO144443_4B:
                {
                    var decrypted = _pn532?.TryDecodeData106kbpsTypeB(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted: Type B {decrypted.ToJson()}");
                    yield return Encoding.UTF8.GetString(decrypted?.NfcId ?? []);
                    break;
                }
                case PollingType.MifareCard:
                {
                    var decrypted = _pn532?.TryDecode106kbpsTypeA(retData.AsSpan()[3..]);
                    logger.LogGuardRailDebug($"NFC Task: Decrypted mifare: {decrypted.ToJson()}");
                    yield return Encoding.UTF8.GetString(decrypted?.NfcId ?? []);
                    break;
                }
            }
        }
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
/*public sealed class NfcHardwareManagerOld(
    INfcConfiguration nfcConfiguration,
    ILoggerFactory loggerFactory,
    ILogger<NfcHardwareManagerOld> logger,
    GpioController gpioController)
    : INfcHardwareManager
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private I2cBus? _i2CBus;
    private I2cDevice _i2cDevice;
    private Task? _listener;
    private GpioController _gpioController = gpioController;

    public ValueTask InitAsync()
    {
        LogDispatcher.LoggerFactory = loggerFactory;
        //var i2cSettings = new I2cConnectionSettings(nfcConfiguration.BusId, nfcConfiguration.DeviceAddress);
        //_i2cDevice = I2cDevice.Create(i2cSettings);
        _listener = new TaskFactory()
            .StartNew(() =>
            {
                Functions.nfc_init(out var nfcContext);
                var nfcDevice = Functions.nfc_open(nfcContext, "pn532_i2c:/dev/i2c-1");
                logger.LogGuardRailInformation($"Opened: {nfcDevice.ToJson()}");
                logger.LogGuardRailInformation("Polling");
                var stopwatch = Stopwatch.StartNew();
                nfc_modulation[] nfcModulations = [
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_ISO14443A,
                        nbr = nfc_baud_rate.NBR_106
                    },
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_ISO14443B,
                        nbr = nfc_baud_rate.NBR_106
                    },
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_FELICA,
                        nbr = nfc_baud_rate.NBR_212
                    },
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_FELICA,
                        nbr = nfc_baud_rate.NBR_424
                    },
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_JEWEL,
                        nbr = nfc_baud_rate.NBR_106
                    },
                    new nfc_modulation
                    {
                        nmt = nfc_modulation_type.NMT_ISO14443BI,
                        nbr = nfc_baud_rate.NBR_106
                    }
                ];
                var modulationsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(nfc_modulation)) * nfcModulations.Length);
                Marshal.StructureToPtr(nfcModulations, modulationsPtr, true);
                var result = Functions.nfc_initiator_poll_target(
                    nfcDevice,
                    modulationsPtr,
                    (uint)nfcModulations.Length,
                    1,
                    1,
                    out var nfcTarget);
                stopwatch.Stop();
                logger.LogGuardRailInformation($"Polled ({result}): {stopwatch.Elapsed:c}");
                if (result < 0)
                {
                    Functions.nfc_perror(
                        nfcDevice,
                        "nfc_initiator_poll_target");
                }

                Functions.str_nfc_target(out var buff, nfcTarget, true);
                logger.LogGuardRailInformation("nfc_target: " + (nfcTarget.Equals(default(nfc_target)) ? "default" : "new") + "~~~" + nfcTarget.ToJson());
                logger.LogGuardRailInformation("buffer: " + buff);
                using var device = _i2CBus.CreateDevice(0x70);
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var buffer = new Span<byte>(new byte[64]);
                    device.Read(
                        buffer);
                    logger.LogGuardRailInformation(Encoding.UTF8.GetString(buffer));
                }
            },
            _cancellationTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        return ValueTask.CompletedTask;
    }

    public event Func<string, CancellationToken, ValueTask>? Submit;

    public int pn53x_initiator_poll_target(
        nfc_device pnd,
        nfc_modulation[] pnmModulations,
        int szModulations,
        byte uiPollNr,
        byte uiPeriod,
        ref nfc_target pnt)
    {
        int res = 0;
        int szTargetTypes = 0;
        PN53xTargetType[] apttTargetTypes = new PN53xTargetType[32];
        Array.Fill(apttTargetTypes, PN53xTargetType.PTT_UNDEFINED);

        for (int n = 0; n < szModulations; n++)
        {
            /*PN53xTargetType ptt = PN53xNMToPTT(pnmModulations[n]);

            if (ptt == PN53xTargetType.PTT_UNDEFINED)
            {
                pnd.last_error = (int) NFCErrorCode.NFC_EINVARG;
                return pnd.last_error;
            }

            apttTargetTypes[szTargetTypes] = ptt;

            if (pnd.bAutoIso14443_4 && ptt == PN53xTargetType.PTT_MIFARE)
            {
                apttTargetTypes[szTargetTypes] = PN53xTargetType.PTT_ISO14443_4A_106;
                szTargetTypes++;
                apttTargetTypes[szTargetTypes] = PN53xTargetType.PTT_MIFARE;
            }#1#

            szTargetTypes++;
        }

        nfc_target[] ntTargets = new nfc_target[2];

        /*if ((res = pn53x_InAutoPoll(pnd, apttTargetTypes, szTargetTypes, uiPollNr, uiPeriod, ntTargets, 0)) < 0)
            return res;#1#

        switch (res)
        {
            case 0:
                return pnd.last_error = (int) NFCErrorCode.NFC_SUCCESS;
            case 1:
                pnt = ntTargets[0];
                if (pn53x_current_target_new(pnd, ref pnt) == null)
                {
                    return pnd.last_error = (int) NFCErrorCode.NFC_ESOFT;
                }

                return res;
            case 2:
                pnt = ntTargets[1];
                if (pn53x_current_target_new(pnd, ref pnt) == null)
                {
                    return pnd.last_error = (int) NFCErrorCode.NFC_ESOFT;
                }

                return res;
            default:
                return (int) NFCErrorCode.NFC_ECHIP;
        }

        return (int) NFCErrorCode.NFC_ECHIP;
    }

    public IntPtr pn53x_current_target_new(nfc_device pnd, ref nfc_target pnt)
    {
        throw new NotImplementedException("pn53x_current_target_new not implemented");
        /#1#/ Keep the current nfc_target for further commands
        if (pnd.chip_data.CurrentTarget != null)
        {
            Marshal.FreeHGlobal(Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0));
        }

        pnd.chip_data.CurrentTarget = new nfc_target()
        {
            Data = new byte[pnt.Data.Length]
        };

        Marshal.Copy(pnt.Data, 0, Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0), pnt.Data.Length);

        return Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0);#1#
    }

    public enum PN53xTargetType
    {
        PTT_UNDEFINED,
        PTT_MIFARE,
        PTT_ISO14443_4A_106,
        // Add other possible target types
    }

    public enum NFCErrorCode
    {
        NFC_SUCCESS,
        NFC_EINVARG,
        NFC_ESOFT,
        NFC_ETIMEOUT,
        NFC_ECHIP,
        // Add other possible error codes
    }

    public ValueTask DisposeAddressAsync(
        int address)
    {
        _cancellationTokenSource.Cancel();
        _listener?.Dispose();
        _i2CBus?.Dispose();
        return ValueTask.CompletedTask;
    }
}*/