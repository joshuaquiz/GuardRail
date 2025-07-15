using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Nfc;

namespace GuardRail.Mobile.Client;

internal class NdefMessageParser
{
    public static IEnumerable<string> Parse(
        NdefMessage message) =>
        message
            .GetRecords()
            ?.Select(x =>
            {
                if (UriRecordParser.TryParseUriRecord(x, out var uriRecord))
                {
                    return uriRecord;
                }

                if (TextRecordParser.TryParseTextRecord(x, out var textRecord))
                {
                    return textRecord;
                }

                if (SmartPosterParser.TryParseSmartPosterRecord(x, out var smartPoster))
                {
                    return smartPoster;
                }

                return Encoding.Default.GetString(
                    (byte[]) (x.GetPayload()
                              ?? []));
            })
        ?? [];
}