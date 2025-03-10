using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// An account in the system.
/// </summary>
public class Account
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The name of the account.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Whether the account is active.
    /// </summary>
    public required bool IsActive { get; set; }
}