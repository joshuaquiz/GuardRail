using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace GuardRail.Core.Helpers
{
    /// <summary>
    /// Extensions!
    /// </summary>
    public static class Extensions
    {

        #region Serialization Extensions

        /// <summary>
        /// Outputs the object as a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the item to serialize.</typeparam>
        /// <param name="item">The item to serialize.</param>
        /// <param name="jsonSerializerSettings">Defaulted to only ignores nulls.</param>
        /// <returns>string</returns>
        public static string ToJson<T>(
            this T item,
            JsonSerializerSettings jsonSerializerSettings = null) =>
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
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new VersionConverter());
            return jsonSerializer.Deserialize<T>(reader);
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
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new VersionConverter());
            return jsonSerializer.Deserialize<T>(reader);
        }

        #endregion

        /// <summary>
        /// Converts one type to another type.
        /// </summary>
        /// <typeparam name="TOut">The type to return.</typeparam>
        /// <param name="from">The value to convert.</param>
        /// <returns>TOut</returns>
        public static TOut Convert<TOut>(this string from)
        {
            if (from == null)
            {
                return default;
            }

            return (TOut)System.Convert.ChangeType(
                from,
                Nullable.GetUnderlyingType(typeof(TOut)) ?? typeof(TOut),
                CultureInfo.InvariantCulture);
        }
    }
}