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

        /// <summary>
        /// Parses a CommandLineArgument from a string
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static CommandLineArgument Parse(string argument)
        {
            if (argument == null || !Regex.IsMatch(argument, "/[a-z0-9]+(?: [a-z0-9\\-\\._])?", RegexOptions.IgnoreCase))
            {
                throw new InvalidCommandLineArgumentFormatException(argument);
            }

            var arg = argument.Split(" ");
            return Enum.TryParse(typeof(CommandLineArgumentType), arg[0][1..], true, out var type)
                ? new CommandLineArgument
                {
                    Type = (CommandLineArgumentType) type,
                    Value = arg.Length == 2
                        ? arg[1]
                        : null
                }
                : throw new InvalidCommandLineArgumentException(arg[0]);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"/{Type:G} {Value}";
    }
}