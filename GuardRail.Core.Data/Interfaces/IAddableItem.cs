using System;

namespace GuardRail.Core.Data.Interfaces;

/// <summary>
/// An item that can be added.
/// </summary>
public interface IAddableItem
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }
}