namespace GuardRail.Core.Enums;

/// <summary>
/// Status of a command.
/// </summary>
public enum CommandStatus : short
{
    /// <summary>
    /// The command has not been picked up.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The command is in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// The command completed with no errors.
    /// </summary>
    CompletedSuccessfully = 2,

    /// <summary>
    /// The command completed with errors.
    /// </summary>
    CompletedWithErrors = 3,

    /// <summary>
    /// The command failed to complete.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// The command expired before it was picked up.
    /// </summary>
    Expired = 5
}