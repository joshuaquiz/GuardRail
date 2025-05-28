using System;
using System.Linq;
using Android.Nfc;
using AndroidX.Core.Util;

namespace GuardRail.Mobile.Client;

internal static class SmartPosterParser
{
    internal static bool TryParseSmartPosterRecord(
        this NdefRecord record,
        out string smartPoster)
    {
        smartPoster = string.Empty;
        try
        {
            Preconditions.CheckArgument(
                record.Tnf == NdefRecord.TnfWellKnown);
            var typeInfo = record
                .GetTypeInfo();
            if (typeInfo == null
                || NdefRecord.RtdSmartPoster == null
                || typeInfo[0] != NdefRecord.RtdSmartPoster[0])
            {
                throw new Exception();
            }

            var subRecords = new NdefMessage(record.GetPayload());
            var records = subRecords.GetRecords()
                ?.Select(x =>
                {
                    var isUri = UriRecordParser.TryParseUriRecord(x, out var uri);
                    var isText = TextRecordParser.TryParseTextRecord(x, out var text);
                    return new
                    {
                        IsUri = isUri,
                        Uri = uri,
                        IsText = isText,
                        Text = text
                    };
                })
                .ToList();
            var title = records
                            ?.FirstOrDefault(
                                x =>
                                    x.IsText)
                            ?.Text
                        ?? string.Empty;
            var uris = records
                           ?.Where(
                               x =>
                                   x.IsUri)
                           .Select(
                               x =>
                                   x.Uri)
                           .ToList()
                       ?? [];
            smartPoster = string.Join(
                              ' ',
                              uris)
                          + ' '
                          + title;
            return true;
        }
        catch
        {
            return false;
        }
    }
}