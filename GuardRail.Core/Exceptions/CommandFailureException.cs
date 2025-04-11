using System;
using GuardRail.Core.Helpers;

namespace GuardRail.Core.Exceptions;

/// <summary>
/// The command failed for the provided reason.
/// </summary>
public sealed class CommandFailureException : GuardRailExceptionBase
{
    public CommandFailureException(
        Guid commandId,
        string reason,
        string? details = null)
        : base(
            $"Command {commandId} failed: {reason}{(details.IsNullOrWhiteSpace() ? null : $"{Environment.NewLine}{details}")}")
    {
        ExternalMessage = $"The command failed \"{reason}\"";
    }
}