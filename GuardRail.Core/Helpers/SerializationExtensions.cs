using System.IO;
using Newtonsoft.Json;

namespace GuardRail.Core.Helpers
{
    /// <summary>
    /// Extensions for serialization and deserialization.
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Outputs the object as a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the item to serialize.</typeparam>
        /// <param name="item">The item to serialize.</param>
        /// <param name="jsonSerializerSettings">Defaulted to only ignores nulls.</param>
        /// <returns>string</returns>
        public static string ToJson<T>(this T item, JsonSerializerSettings jsonSerializerSettings = null) =>
            JsonConvert.SerializeObject(
                item,
                jsonSerializerSettings
                ?? new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

        /// <summary>
        /// Deserialize JSON to type.
        /// </summary>
        /// <typeparam name="T">The type of the item to deserialize.</typeparam>
        /// <param name="s">The JSON string.</param>
        /// <returns>T</returns>
        public static T FromJson<T>(this string s)
        {
            using var stringReader = new StringReader(s);
            using JsonReader reader = new JsonTextReader(stringReader);
            return new JsonSerializer()
                .Deserialize<T>(reader);
        }

        /// <summary>
        /// Deserialize JSON to type.
        /// </summary>
        /// <typeparam name="T">The type of the item to deserialize.</typeparam>
        /// <param name="s">The JSON string.</param>
        /// <returns>T</returns>
        public static T FromJson<T>(this Stream s)
        {
            using var streamReader = new StreamReader(s);
            using JsonReader reader = new JsonTextReader(streamReader);
            return new JsonSerializer()
                .Deserialize<T>(reader);
        }
    }
}