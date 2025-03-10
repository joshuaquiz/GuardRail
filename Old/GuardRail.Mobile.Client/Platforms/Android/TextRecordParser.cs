using System;
using System.Text;
using Android.Nfc;
using AndroidX.Core.Util;

namespace GuardRail.Mobile.Client;

internal static class TextRecordParser
{
    internal static bool TryParseTextRecord(
        this NdefRecord record,
        out string text)
    {
        text = string.Empty;
        try
        {
            Preconditions.CheckArgument(record.Tnf == NdefRecord.TnfWellKnown);
            var typeInfo = record.GetTypeInfo();
            if (typeInfo == null
                || NdefRecord.RtdText == null
                || typeInfo[0] != NdefRecord.RtdText[0])
            {
                throw new Exception();
            }

            var payload = record
                              .GetPayload()
                          ?? [0x00];
            var textEncoding = (payload[0] & 0200) == 0
                ? "UTF-8"
                : "UTF-16";
            var languageCodeLength = payload[0] & 0077;
            text = Encoding.GetEncoding(
                    textEncoding)
                .GetString(
                    payload,
                    languageCodeLength + 1,
                    payload.Length - languageCodeLength - 1);
            return true;
        }
        catch
        {
            return false;
        }
    }
}