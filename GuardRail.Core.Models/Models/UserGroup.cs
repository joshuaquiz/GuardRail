using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A group of users in the system.
/// </summary>
public class UserGroup
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public required Guid Guid { get; set; }

    /// <summary>
    /// The ID of the account.
    /// </summary>
    public required Guid AccountGuid { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    public required string Name { get; set; }
}