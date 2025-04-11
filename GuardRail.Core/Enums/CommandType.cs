namespace GuardRail.Core.Enums;

/// <summary>
/// Types of commands.
/// </summary>
public enum CommandType : long
{
    /// <summary>
    /// A basic ping command.
    /// </summary>
    Ping = 0,

    /// <summary>
    /// A command to list all access points of a type connected to the local system.
    /// </summary>
    GetAvailableAccessPoints = 1
}