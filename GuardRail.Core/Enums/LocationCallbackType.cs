namespace GuardRail.Core.Enums;

/// <summary>
/// Types of methods used by the main system to communicate with local systems.
/// </summary>
public enum LocationCallbackType : byte
{
    /// <summary>
    /// Used to denote an air-gapped system where no external communication is allowed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Used when the local system polls for pending commands.
    /// </summary>
    Polling = 1,

    /// <summary>
    /// Used when the local system is called directly by the main system using a defined open port.
    /// </summary>
    Direct = 2
}