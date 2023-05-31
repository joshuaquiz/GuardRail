using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DeviceLogic.Interfaces.Input.Nfc;
using Iot.Device.Card.CreditCardProcessing;
using Iot.Device.Card.Mifare;
using Iot.Device.Common;
using Iot.Device.Ndef;
using Iot.Device.Pn532;
using Iot.Device.Pn532.ListPassive;
using Microsoft.Extensions.Logging;
using TagList = Iot.Device.Card.CreditCardProcessing.TagList;

namespace GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.Input.Nfc;

public sealed class NfcHardwareManager : INfcHardwareManager
{
    private readonly INfcConfiguration _nfcConfiguration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<NfcHardwareManager> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private Pn532? _pn532;
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
        _pn532 = new Pn532(_nfcConfiguration.SerialPort);
        _listener = new TaskFactory()
            .StartNew(
                () =>
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        var nfcAutoPollData = new NfcAutoPollData(_pn532.AutoPoll(5, 300, new[] { PollingType.Passive106kbpsISO144443_4A, PollingType.Passive106kbpsISO144443_4B }));
                        _logger.LogGuardRailInformation($"Has Targets: {nfcAutoPollData.HasTargets}, Num tags: {nfcAutoPollData.NumberOfTargets}, Type: {nfcAutoPollData.TargetType}");
                        if (nfcAutoPollData.HasTargets)
                        {
                            switch (nfcAutoPollData.TargetType)
                            {
                                case PollingType.GenericPassive106kbps:
                                    break;
                                case PollingType.GenericPassive212kbps:
                                    break;
                                case PollingType.GenericPassive424kbps:
                                    break;
                                case PollingType.Passive106kbps:
                                    break;
                                case PollingType.InnovisionJewel:
                                    break;
                                case PollingType.MifareCard:
                                    break;
                                case PollingType.Felica212kbps:
                                    break;
                                case PollingType.Felica424kbps:
                                    break;
                                case PollingType.Passive106kbpsISO144443_4A:
                                {
                                    var card = _pn532.TryDecode106kbpsTypeA(nfcAutoPollData.CardData);
                                    if (card is null)
                                    {
                                        Debug.WriteLine("Can't read properly the card");
                                        return;
                                    }

                                    // Create the Mifare card
                                    var mifareCard = new MifareCard(_pn532, card.TargetNumber) { BlockNumber = 0, Command = MifareCardCommand.AuthenticationA };
                                    mifareCard.SetCapacity(card.Atqa, card.Sak);
                                    mifareCard.SerialNumber = card.NfcId;
                                    // Read an extract the NDEF message
                                    // This is where you can write as well, format the card, check the card see next sections
                                    mifareCard.TryReadNdefMessage(out var message);

                                    if (message.Records.Count == 0)
                                    {
                                        Debug.WriteLine("Sorry, there is no NDEF message in this card or I can't find them");
                                    }

                                    // Display the messages
                                    foreach (var msg in message.Records)
                                    {
                                        Debug.WriteLine("Record header:");
                                        Debug.WriteLine($"  Is first message: {msg.Header.IsFirstMessage}, is last message: {msg.Header.IsLastMessage}");
                                        Debug.Write($"  Type name format: {msg.Header.TypeNameFormat}");
                                        if (msg.Header.PayloadType != null)
                                        {
                                            Debug.WriteLine($", Payload type: {BitConverter.ToString(msg.Header.PayloadType)}");
                                        }
                                        else
                                        {
                                            Debug.WriteLine("");
                                        }

                                        Debug.WriteLine($"  Is composed: {msg.Header.IsComposedMessage}, is Id present: {msg.Header.MessageFlag.HasFlag(MessageFlag.IdLength)}, Id Length value: {msg.Header.IdLength}");
                                        Debug.WriteLine($"  Payload Length: {msg.Payload?.Length}, is short message= {msg.Header.MessageFlag.HasFlag(MessageFlag.ShortRecord)}");

                                        if (msg.Payload != null)
                                        {
                                            Debug.WriteLine($"Payload: {BitConverter.ToString(msg.Payload)}");
                                        }
                                        else
                                        {
                                            Debug.WriteLine("No payload");
                                        }

                                        if (UriRecord.IsUriRecord(msg))
                                        {
                                            var urirec = new UriRecord(msg);
                                            Debug.WriteLine($"  Type {nameof(UriRecord)}, Uri Type: {urirec.UriType}, Uri: {urirec.Uri}, Full URI: {urirec.FullUri}");
                                        }

                                        if (TextRecord.IsTextRecord(msg))
                                        {
                                            var txtrec = new TextRecord(msg);
                                            Debug.WriteLine($"  Type: {nameof(TextRecord)}, Encoding: {txtrec.Encoding}, Language: {txtrec.LanguageCode}, Text: {txtrec.Text}");
                                        }

                                        if (GeoRecord.IsGeoRecord(msg))
                                        {
                                            var geo = new GeoRecord(msg);
                                            Debug.WriteLine($"  Type: {nameof(GeoRecord)}, Lat: {geo.Latitude}, Long: {geo.Longitude}");
                                        }

                                        if (MediaRecord.IsMediaRecord(msg))
                                        {
                                            var media = new MediaRecord(msg);
                                            Debug.WriteLine($"  Type: {nameof(MediaRecord)}, Payload Type = {media.PayloadType}");
                                            if (media.IsTextType)
                                            {
                                                var ret = media.TryGetPayloadAsText(out var payloadAsText);
                                                if (ret)
                                                {
                                                    Debug.WriteLine($"    Payload as Text:");
                                                    Debug.WriteLine($"{payloadAsText}");
                                                }
                                                else
                                                {
                                                    Debug.WriteLine($"Can't convert the payload as a text");
                                                }
                                            }
                                        }

                                        Debug.WriteLine("");
                                    }
                                    break;
                                }
                                case PollingType.Passive106kbpsISO144443_4B:
                                {
                                    var decrypted = _pn532.TryDecodeData106kbpsTypeB(nfcAutoPollData.CardData);
                                    if (decrypted is null)
                                    {
                                        Debug.WriteLine("Can't read properly the card");
                                        return;
                                    }

                                    _logger.LogGuardRailInformation(
                                        $"{decrypted.TargetNumber}, Serial: {BitConverter.ToString(decrypted.NfcId)}, App Data: {BitConverter.ToString(decrypted.ApplicationData)}, " +
                                        $"{decrypted.ApplicationType}, Bit Rates: {decrypted.BitRates}, CID {decrypted.CidSupported}, Command: {decrypted.Command}, FWT: {decrypted.FrameWaitingTime}, " +
                                        $"ISO144443 compliance: {decrypted.ISO14443_4Compliance}, Max Frame size: {decrypted.MaxFrameSize}, NAD: {decrypted.NadSupported}");

                                    var creditCard = new CreditCard(_pn532, decrypted.TargetNumber);
                                    creditCard.ReadCreditCardInformation();

                                    _logger.LogGuardRailInformation("All Tags for the Credit Card:");
                                    DisplayTags(creditCard.Tags, 0);
                                    break;
                                }
                                case PollingType.DepPassive106kbps:
                                    break;
                                case PollingType.DepPassive212kbps:
                                    break;
                                case PollingType.DepPassive424kbps:
                                    break;
                                case PollingType.DepActive106kbps:
                                    break;
                                case PollingType.DepActive212kbps:
                                    break;
                                case PollingType.DepActive424kbps:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                },
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        return ValueTask.CompletedTask;
    }

    private static string AddSpace(int level)
    {
        var space = string.Empty;
        for (var i = 0; i < level; i++)
        {
            space += "  ";
        }

        return space;
    }

    private void DisplayTags(List<Tag> tagToDisplay, int levels)
    {
        foreach (var tagParent in tagToDisplay)
        {
            Console.Write(AddSpace(levels) + $"{tagParent.TagNumber:X4}-{TagList.Tags.FirstOrDefault(m => m.TagNumber == tagParent.TagNumber)?.Description}");
            var isTemplate = TagList.Tags.FirstOrDefault(m => m.TagNumber == tagParent.TagNumber);
            if (isTemplate?.IsTemplate == true || isTemplate?.IsConstructed == true)
            {
                _logger.LogGuardRailInformation(string.Empty);
                DisplayTags(tagParent.Tags, levels + 1);
            }
            else if (isTemplate?.IsDol == true)
            {
                // In this case, all the data inside are 1 byte only
                _logger.LogGuardRailInformation(", Data Object Length elements:");
                foreach (var dt in tagParent.Tags)
                {
                    Console.Write(AddSpace(levels + 1) + $"{dt.TagNumber:X4}-{TagList.Tags.FirstOrDefault(m => m.TagNumber == dt.TagNumber)?.Description}");
                    _logger.LogGuardRailInformation($", data length: {dt.Data[0]}");
                }
            }
            else
            {
                var tg = new TagDetails(tagParent);
                _logger.LogGuardRailInformation($": {tg}");
            }
        }
    }

    public event Func<string, CancellationToken, ValueTask>? Submit;

    public ValueTask DisposeAddressAsync(
        string address)
    {
        _cancellationTokenSource.Cancel();
        _listener?.Dispose();
        _pn532?.Dispose();
        return ValueTask.CompletedTask;
    }

    private readonly ref struct NfcAutoPollData
    {
        private readonly Span<byte> _rawData;

        internal NfcAutoPollData(
            Span<byte> data)
        {
            _rawData = data;
        }

        internal bool HasTargets => _rawData.Length >= 3;

        internal int NumberOfTargets => _rawData[0];

        internal PollingType TargetType => (PollingType)_rawData[1];

        internal int LengthOfData => _rawData[2];

        internal Span<byte> CardData => _rawData[3..];
    }
}