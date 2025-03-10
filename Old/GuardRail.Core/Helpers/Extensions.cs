using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GuardRail.Core.Helpers;

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
        JsonSerializerSettings? jsonSerializerSettings = null) =>
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
    public static T? FromJson<T>(this string s)
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
    public static T? FromJson<T>(this Stream s)
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
    public static TOut? Convert<TOut>(this string? from)
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

    /// <summary>
    /// Extension for IsNullOrWhiteSpace.
    /// </summary>
    /// <param name="str">The value to use.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str) =>
        string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// Extension for IsNullOrEmpty.
    /// </summary>
    /// <param name="str">The value to use.</param>
    /// <returns>bool</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str) =>
        string.IsNullOrEmpty(str);

    /// <summary>
    /// Gets the UTF8 encoded bytes from the provided text.
    /// </summary>
    /// <param name="str">The value to use.</param>
    /// <returns><see cref="byte"/>s</returns>
    public static byte[] GetBytes(this string? str) =>
        str.IsNullOrEmpty()
            ? []
            : Encoding.UTF8.GetBytes(str);

    /// <summary>
    /// Logs an exception with meta-data.
    /// </summary>
    /// <param name="logger">The logger to write the exception to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="additionalMessage">Any additional information to log, the exception's message is automatically logged.</param>
    /// <param name="memberName">DO NOT USE: Auto-populated by <see cref="CallerMemberNameAttribute"/>.</param>
    /// <param name="sourceFilePath">DO NOT USE: Auto-populated by <see cref="CallerFilePathAttribute"/>.</param>
    /// <param name="sourceLineNumber">DO NOT USE: Auto-populated by <see cref="CallerLineNumberAttribute"/>.</param>
    /// <typeparam name="TLoggerType">The logger type.</typeparam>
    /// <typeparam name="TException">The exception type.</typeparam>
    public static void LogGuardRailError<TLoggerType, TException>(
        this ILogger<TLoggerType> logger,
        TException exception,
        string? additionalMessage = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
        where TException : Exception =>
        logger.LogError(
            exception,
            $"{Path.GetFileName(sourceFilePath)}:{sourceLineNumber}({memberName}) {(additionalMessage.IsNullOrWhiteSpace() ? string.Empty : additionalMessage + " ")}{exception.Message}");

    /// <summary>
    /// Logs an informational message with meta-data.
    /// </summary>
    /// <param name="logger">The logger to write the informational log to.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">DO NOT USE: Auto-populated by <see cref="CallerMemberNameAttribute"/>.</param>
    /// <param name="sourceFilePath">DO NOT USE: Auto-populated by <see cref="CallerFilePathAttribute"/>.</param>
    /// <param name="sourceLineNumber">DO NOT USE: Auto-populated by <see cref="CallerLineNumberAttribute"/>.</param>
    /// <typeparam name="TLoggerType">The logger type.</typeparam>
    public static void LogGuardRailInformation<TLoggerType>(
        this ILogger<TLoggerType> logger,
        string? message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) =>
        logger.LogInformation(
            $"{Path.GetFileName(sourceFilePath)}:{sourceLineNumber}({memberName}): {message}");
}