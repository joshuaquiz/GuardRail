using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A method users in a user group can use to gain access to an access point.
/// </summary>
public class UserGroupAccessMethod : AccessPointAccessMethodBase
{
    /// <summary>
    /// The ID of the user group.
    /// </summary>
    public required Guid UserGroupGuid { get; set; }
}