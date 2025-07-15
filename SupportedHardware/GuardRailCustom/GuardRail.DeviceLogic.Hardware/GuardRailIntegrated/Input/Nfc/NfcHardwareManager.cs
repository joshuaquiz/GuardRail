using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc.NfcTools.PInvoke;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Iot.Device.Card.CreditCardProcessing;
using Iot.Device.Card.Mifare;
using Iot.Device.Common;
using Iot.Device.FtCommon;
using Iot.Device.Ndef;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using ManagedLibnfc;
using ManagedLibnfc.PInvoke;
using Microsoft.Extensions.Logging;
using TagList = Iot.Device.Card.CreditCardProcessing.TagList;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc;

public sealed class NfcHardwareManager : INfcHardwareManager
{
    private readonly INfcConfiguration _nfcConfiguration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<NfcHardwareManager> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private I2cBus? _i2CBus;
    private Task? _listener;
    private GpioController _gpioController;

    public NfcHardwareManager(
        INfcConfiguration nfcConfiguration,
        ILoggerFactory loggerFactory,
        ILogger<NfcHardwareManager> logger,
        GpioController gpioController)
    {
        _nfcConfiguration = nfcConfiguration;
        _loggerFactory = loggerFactory;
        _logger = logger;
        _gpioController = gpioController;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public ValueTask InitAsync()
    {
        LogDispatcher.LoggerFactory = _loggerFactory;
        _i2CBus = I2cBus.Create(int.Parse(_nfcConfiguration.SerialPort));
        _listener = new TaskFactory()
            .StartNew(() =>
            {
                Functions.nfc_init(out var nfcContext);
                var nfcDevice = Functions.nfc_open(nfcContext, "pn532_i2c:/dev/i2c-1");
                _logger.LogGuardRailInformation($"Opened: {nfcDevice.ToJson()}");
                _logger.LogGuardRailInformation("Polling");
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
                _logger.LogGuardRailInformation($"Polled ({result}): {stopwatch.Elapsed:c}");
                if (result < 0)
                {
                    Functions.nfc_perror(
                        nfcDevice,
                        "nfc_initiator_poll_target");
                }

                Functions.str_nfc_target(out var buff, nfcTarget, true);
                _logger.LogGuardRailInformation("nfc_target: " + (nfcTarget.Equals(default(nfc_target)) ? "default" : "new") + "~~~" + nfcTarget.ToJson());
                _logger.LogGuardRailInformation("buffer: " + buff);
                using var device = _i2CBus.CreateDevice(0x70);
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var buffer = new Span<byte>(new byte[64]);
                    device.Read(
                        buffer);
                    _logger.LogGuardRailInformation(Encoding.UTF8.GetString(buffer));
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
            PN53xTargetType ptt = PN53xNMToPTT(pnmModulations[n]);

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
            }

            szTargetTypes++;
        }

        nfc_target[] ntTargets = new nfc_target[2];

        if ((res = pn53x_InAutoPoll(pnd, apttTargetTypes, szTargetTypes, uiPollNr, uiPeriod, ntTargets, 0)) < 0)
            return res;

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
        // Keep the current nfc_target for further commands
        if (pnd.chip_data.CurrentTarget != null)
        {
            Marshal.FreeHGlobal(Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0));
        }

        pnd.chip_data.CurrentTarget = new nfc_target()
        {
            Data = new byte[pnt.Data.Length]
        };

        Marshal.Copy(pnt.Data, 0, Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0), pnt.Data.Length);

        return Marshal.UnsafeAddrOfPinnedArrayElement(pnd.chip_data.CurrentTarget.Data, 0);
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
        string address)
    {
        _cancellationTokenSource.Cancel();
        _listener?.Dispose();
        _i2CBus?.Dispose();
        return ValueTask.CompletedTask;
    }
}