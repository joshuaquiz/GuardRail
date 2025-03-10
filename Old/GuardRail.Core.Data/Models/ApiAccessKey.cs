using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A user's API access key.
/// </summary>
public class ApiAccessKey : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The time the key expires.
    /// </summary>
    [Required]
    public DateTimeOffset Expiry { get; set; }

    /// <summary>
    /// The Guid of the <see cref="Models.User"/>.
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// The <see cref="Models.User"/>.
    /// </summary>
    public User User { get; set; } = null!;
}