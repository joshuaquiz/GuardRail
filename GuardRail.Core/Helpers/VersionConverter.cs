using System;
using System.Linq;
using Newtonsoft.Json;

namespace GuardRail.Core.Helpers;

/// <inheritdoc/>
public class VersionConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer) =>
        serializer?.Serialize(writer, value);

    /// <inheritdoc/>
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        if (serializer == null)
        {
            return new Version();
        }

        var sections = serializer.Deserialize<string>(reader).Split('.');
        return new Version(
            sections.ElementAtOrDefault(0).Convert<int>(),
            sections.ElementAtOrDefault(1).Convert<int>(),
            sections.ElementAtOrDefault(2).Convert<int>(),
            sections.ElementAtOrDefault(3).Convert<int>());
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) =>
        objectType == typeof(Version);
}