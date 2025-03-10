using System;
using GuardRail.Core.Models.Enums;

namespace GuardRail.Api.Models.Requests;

/// <summary>
/// An API request to unlock a door.
/// </summary>
/// <param name="AccessPointGuid">The guid of the access point.</param>
/// <param name="UnlockTriggerType">The method used to attempt to access the door.</param>
/// <param name="Data">The data used.</param>
/// <param name="UnlockRequestToken">The ID of the unlock request token.</param>
public sealed record UnLockRequest(
    Guid AccessPointGuid,
    UnlockTriggerType UnlockTriggerType,
    byte[] Data,
    Guid UnlockRequestToken);