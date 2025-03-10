using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// Represents a user access token.
/// </summary>
public class UserAccessToken
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the user the token is for.
    /// </summary>
    public required Guid UserGuid { get; set; }

    /// <summary>
    /// The timestamp the token was created.
    /// </summary>
    public required DateTimeOffset DateTime { get; set; }

    /// <summary>
    /// The Duration the token is valid for.
    /// </summary>
    public required TimeSpan Duration { get; set; }

    /// <summary>
    /// Whether the token is still active.
    /// </summary>
    public required bool IsActive { get; set; }
}