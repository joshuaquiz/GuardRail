using System;
using System.Text.RegularExpressions;
using GuardRail.Core.Helpers;

namespace GuardRail.Core.CommandLine;

/// <summary>
/// A single command line argument.
/// </summary>
public sealed class CommandLineArgument
{
    /// <summary>
    /// The type of the argument.
    /// </summary>
    public CommandLineArgumentType Type { get; private init; }

    /// <summary>
    /// The value of the argument as a string.
    /// </summary>
    public string? Value { get; private init; }

    /// <summary>
    /// Parses a CommandLineArgument from a string
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    public static CommandLineArgument Parse(string? argument)
    {
        if (argument == null || !Regex.IsMatch(argument, "/[a-z0-9]+(?: [a-z0-9\\-\\._\"])?", RegexOptions.IgnoreCase))
        {
            throw new InvalidCommandLineArgumentFormatException(argument ?? string.Empty);
        }

        var indexOfSpace = argument.IndexOf(" ", StringComparison.InvariantCultureIgnoreCase);
        var arg = indexOfSpace > -1
            ? argument[1..indexOfSpace]
            : argument[1..];
        var val = indexOfSpace > -1
            ? argument[(indexOfSpace+1)..].Trim('"')
            : null;
        return Enum.TryParse(typeof(CommandLineArgumentType), arg, true, out var type)
            ? new CommandLineArgument
            {
                Type = (CommandLineArgumentType) type,
                Value = val
            }
            : throw new InvalidCommandLineArgumentException(arg);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var type = Type.ToString();
        string? value;
        if (Value.IsNullOrWhiteSpace())
        {
            value = null;
        }
        else if (Value?.Contains(' ', StringComparison.InvariantCultureIgnoreCase) == true)
        {
            value = $" \"{Value}\"";
        }
        else
        {
            value = $" {Value}";
        }
        return $"/{type}{value}";
    }
}