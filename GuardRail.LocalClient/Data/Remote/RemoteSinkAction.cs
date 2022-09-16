using System;

namespace GuardRail.LocalClient.Data.Remote;

/// <summary>
/// Defines a sink action.
/// </summary>
public sealed class RemoteSinkAction
{
    /// <summary>
    /// The type of action.
    /// </summary>
    public RemoteSinkActionType RemoteSinkActionType { get; set; }

    /// <summary>
    /// The type of the item.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// The item.
    /// </summary>
    public object Item { get; set; }
}