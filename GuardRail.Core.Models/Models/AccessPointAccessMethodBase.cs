using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A method used to gain access to an access point.
/// </summary>
public abstract class AccessPointAccessMethodBase
{
    /// <summary>
    /// The ID of the access point.
    /// </summary>
    public required Guid AccessPointGuid { get; set; }

    /// <summary>
    /// The method used for the access.
    /// </summary>
    public required UnlockTriggerType UnlockTriggerType { get; set; }

    /// <summary>
    /// The time of day to start allowing access.
    /// </summary>
    public TimeOnly? StartTime { get; set; }

    /// <summary>
    /// The time of day to no longer allow access.
    /// </summary>
    public TimeOnly? EndTime { get; set; }

    /// <summary>
    /// The date to start allowing access.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// The date to no longer allow access.
    /// </summary>
    public DateOnly? EndDate { get; set; }
}