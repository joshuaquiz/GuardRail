using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A method a user can use to gain access to an access point.
/// </summary>
public class UserAccessMethod : AccessPointAccessMethodBase
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    public required Guid UserGuid { get; set; }
}