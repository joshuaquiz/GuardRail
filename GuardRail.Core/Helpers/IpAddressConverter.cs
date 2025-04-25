using System;
using System.Net;
using Newtonsoft.Json;

namespace GuardRail.Core.Helpers;

/// <inheritdoc/>
public class IpAddressConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer) =>
        serializer.Serialize(writer, value is IPAddress ipAddress ? ipAddress.ToString() : IPAddress.None.ToString());

    /// <inheritdoc/>
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var sections = serializer.Deserialize<string>(reader);
        return IPAddress.Parse(sections);
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) =>
        objectType == typeof(IPAddress);
}