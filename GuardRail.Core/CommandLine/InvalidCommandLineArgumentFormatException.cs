using System;

namespace GuardRail.Core.CommandLine;

/// <summary>
/// The exception happens when a command line argument is not well formatted.
/// </summary>
public sealed class InvalidCommandLineArgumentFormatException : Exception
{
    /// <summary>
    /// The invalid argument that was passed in.
    /// </summary>
    public string InvalidArgument { get; }

    /// <summary>
    /// The exception happens when a command line argument is not well formatted.
    /// </summary>
    /// <param name="invalidArgument"></param>
    public InvalidCommandLineArgumentFormatException(string invalidArgument)
        : base($"\"{invalidArgument}\" is not correctly formatted")
    {
        InvalidArgument = invalidArgument;
    }
}