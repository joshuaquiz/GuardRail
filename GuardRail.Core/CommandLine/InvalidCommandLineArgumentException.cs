using System;

namespace GuardRail.Core.CommandLine;

/// <summary>
/// The exception happens when a command line argument is not in the defined list of arguments.
/// </summary>
public sealed class InvalidCommandLineArgumentException : Exception
{
    /// <summary>
    /// The invalid argument that was passed in.
    /// </summary>
    public string InvalidArgument { get; }

    /// <summary>
    /// The exception happens when a command line argument is not in the defined list of arguments.
    /// </summary>
    /// <param name="invalidArgument"></param>
    public InvalidCommandLineArgumentException(string invalidArgument)
        : base($"\"{invalidArgument}\" is not a valid argument")
    {
        InvalidArgument = invalidArgument;
    }
}