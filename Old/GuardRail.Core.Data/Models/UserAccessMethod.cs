using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A method a user can gain access.
/// </summary>
public class UserAccessMethod : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The method used for the access method.
    /// </summary>
    [Required]
    public UnlockRequestType UnlockRequestType { get; set; }

    /// <summary>
    /// The data for this type.
    /// </summary>
    /// <remarks>
    /// This can be a phone ID, a pass-code, RFID, etc.
    /// </remarks>
    [Required]
    public byte[] Data { get; set; } = [];

    /// <summary>
    /// The Guid of the <see cref="Models.User"/>.
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// The <see cref="Models.User"/>.
    /// </summary>
    public User User { get; set; } = null!;
}