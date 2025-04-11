using System;

namespace GuardRail.Core.Exceptions;

/// <summary>
/// The result of the command was not in the expected format.
/// </summary>
public sealed class InvalidCommandResponseFormatException : GuardRailExceptionBase
{
    public InvalidCommandResponseFormatException(
        string? response,
        Exception innerException)
        : base(
            $"Invalid response: {response}",
            innerException)
    {
        ExternalMessage = "The result of the command was not in the expected format";
    }
}