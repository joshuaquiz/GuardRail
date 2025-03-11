namespace GuardRail.Core.CommandLine;

/// <summary>
/// All valid command line arguments.
/// </summary>
public enum CommandLineArgumentType
{
    /// <summary>
    /// Default
    /// </summary>
    Default = -1,

    /// <summary>
    /// Denotes that a fresh install should be done.
    /// </summary>
    FreshInstall = 0,

    /// <summary>
    /// Denotes that an update should be done.
    /// </summary>
    Update = 1,

    /// <summary>
    /// Denotes that the main application should show the setup screen.
    /// </summary>
    ShouldShowSetup = 2
}