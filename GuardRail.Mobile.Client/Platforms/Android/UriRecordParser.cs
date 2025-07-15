using System;
using System.Collections.Generic;
using System.Text;
using Android.Nfc;

namespace GuardRail.Mobile.Client;

internal static class UriRecordParser
{
    internal static bool TryParseUriRecord(
        this NdefRecord record,
        out string uri)
    {
        uri = string.Empty;
        try
        {
            var tnf = record.Tnf;
            uri = tnf switch
            {
                NdefRecord.TnfWellKnown => ParseWellKnown(record),
                NdefRecord.TnfAbsoluteUri => ParseAbsolute(record),
                _ => throw new Exception("Unknown tnf")
            };
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ParseAbsolute(
        NdefRecord record)
    {
        var payload = record
            .GetPayload();
        return payload == null
            ? string.Empty
            : Encoding.GetEncoding(
                    "UTF-8")
                .GetString(
                    payload);
    }

    private static string ParseWellKnown(
        NdefRecord record)
    {
        var typeInfo = record
            .GetTypeInfo();
        if (typeInfo == null
            || NdefRecord.RtdUri == null
            || typeInfo[0] != NdefRecord.RtdUri[0])
        {
            throw new Exception();
        }

        var payload = record
                          .GetPayload()
                      ?? [0x00];
        ByteToUrlPrefixMap
            .TryGetValue(
                payload[0],
                out var prefix);
        return prefix
               + Encoding.GetEncoding(
                       "UTF-8")
                   .GetString(
                       payload,
                       1,
                       payload.Length - 1);
    }

    private static readonly Dictionary<byte, string> ByteToUrlPrefixMap = new()
    {
        {0x00, ""},
        {0x01, "http://www."},
        {0x02, "https://www."},
        {0x03, "http://"},
        {0x04, "https://"},
        {0x05, "tel:"},
        {0x06, "mailto:"},
        {0x07, "ftp://anonymous:anonymous@"},
        {0x08, "ftp://ftp."},
        {0x09, "ftps://"},
        {0x0A, "sftp://"},
        {0x0B, "smb://"},
        {0x0C, "nfs://"},
        {0x0D, "ftp://"},
        {0x0E, "dav://"},
        {0x0F, "news:"},
        {0x10, "telnet://"},
        {0x11, "imap:"},
        {0x12, "rtsp://"},
        {0x13, "urn:"},
        {0x14, "pop:"},
        {0x15, "sip:"},
        {0x16, "sips:"},
        {0x17, "tftp:"},
        {0x18, "btspp://"},
        {0x19, "btl2cap://"},
        {0x1A, "btgoep://"},
        {0x1B, "tcpobex://"},
        {0x1C, "irdaobex://"},
        {0x1D, "file://"},
        {0x1E, "urn:epc:id:"},
        {0x1F, "urn:epc:tag:"},
        {0x20, "urn:epc:pat:"},
        {0x21, "urn:epc:raw:"},
        {0x22, "urn:epc:"},
        {0x23, "urn:nfc:"}
    };
}