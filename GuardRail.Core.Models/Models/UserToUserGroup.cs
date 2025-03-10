using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A connection between users and user groups.
/// </summary>
public class UserToUserGroup
{
    /// <summary>
    /// The ID of the User.
    /// </summary>
    public required Guid UserGuid { get; set; }

    /// <summary>
    /// The ID of the group.
    /// </summary>
    public required Guid UserGroupGuid { get; set; }
}