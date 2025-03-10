using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// Represents any restrictions to a login.
/// </summary>
public class Restriction : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The Guid of the <see cref="GrantedAccess"/>.
    /// </summary>
    public Guid GrantedAccessGuid { get; set; }

    /// <summary>
    /// The <see cref="GrantedAccess"/>.
    /// </summary>
    public GrantedAccess GrantedAccess { get; set; } = null!;

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

    /// <summary>
    /// Allowed access types.
    /// </summary>
    public List<UnlockRequestType>? AllowedUnlockRequestTypes { get; set; }

    /// <summary>
    /// Blocked access types.
    /// </summary>
    public List<UnlockRequestType>? BlockedUnlockRequestTypes { get; set; }
}