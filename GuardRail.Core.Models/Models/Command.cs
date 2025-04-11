using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A command in the system.
/// </summary>
public class Command
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the location.
    /// </summary>
    public required Guid LocationGuid { get; set; }

    /// <summary>
    /// The command type.
    /// </summary>
    public required CommandType Type { get; set; }

    /// <summary>
    /// The command status.
    /// </summary>
    public required CommandStatus Status { get; set; }

    /// <summary>
    /// The time the command was created.
    /// </summary>
    public required DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The time the command expires.
    /// </summary>
    public required DateTimeOffset ExpiryDate { get; set; }

    /// <summary>
    /// The maximum number of times the command can be retried before being considered a failure.
    /// </summary>
    public required int MaxRetries { get; set; }

    /// <summary>
    /// The body of the command.
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// The number of times the command has been retried.
    /// </summary>
    public required int Attempts { get; set; }

    /// <summary>
    /// The body of the response to the command.
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// The time the command was started.
    /// </summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// The time the command was completed.
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }
}