using System;
using System.Text.RegularExpressions;

namespace GuardRail.Core.CommandLine
{
    /// <summary>
    /// A single command line argument.
    /// </summary>
    public sealed class CommandLineArgument
    {
        /// <summary>
        /// The type of the argument.
        /// </summary>
        public CommandLineArgumentType Type { get; private set; }

        /// <summary>
        /// The value of the argument as a string.
        /// </summary>
        public string Value { get; set; }

        public static CommandLineArgument Parse(string argument)
        {
            if (argument == null || !Regex.IsMatch(argument, "/[a-z0-9]+ [a-z0-9\\-\\._]", RegexOptions.IgnoreCase))
            {
                throw new InvalidCommandLineArgumentFormatException(argument);
            }

            var arg = argument.Split(" ");
            if (Enum.TryParse(typeof(CommandLineArgumentType), arg[0].Substring(1), true, out var type))
            {
                return new CommandLineArgument
                {
                    Type = (CommandLineArgumentType)type,
                    Value = arg[1]
                };
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"/{Type:G} {Value}";
    }

    public enum CommandLineArgumentType
    {

    }
}