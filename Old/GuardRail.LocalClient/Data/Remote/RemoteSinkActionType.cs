namespace GuardRail.LocalClient.Data.Remote;

/// <summary>
/// The type of sink action.
/// </summary>
public enum RemoteSinkActionType
{
    /// <summary>
    /// Add an item.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Update an item.
    /// </summary>
    Update = 1,

    /// <summary>
    /// Delete an item.
    /// </summary>
    Delete = 2
}