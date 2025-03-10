using System;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// Represents an unlock token.
/// </summary>
public class UnlockAccessToken
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the access point the token is for.
    /// </summary>
    public required Guid AccessPointGuid { get; set; }

    /// <summary>
    /// The timestamp the token was created.
    /// </summary>
    public required DateTimeOffset DateTime { get; set; }

    /// <summary>
    /// The Duration the token is valid for.
    /// </summary>
    public required TimeSpan Duration { get; set; }

    /// <summary>
    /// A GUID returned to the caller on successful access for optional validation.
    /// </summary>
    public required Guid SuccessGuid { get; set; }
}