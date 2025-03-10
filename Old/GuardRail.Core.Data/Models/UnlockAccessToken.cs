using System;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// Represents an unlock token.
/// </summary>
public class UnlockAccessToken : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The ID of the user the token is for.
    /// </summary>
    [Required]
    public Guid UserGuid { get; set; }

    /// <summary>
    /// The timestamp the token was created.
    /// </summary>
    [Required]
    public DateTimeOffset DateTime { get; set; }

    /// <summary>
    /// The Duration the token is valid for.
    /// </summary>
    [Required]
    public TimeSpan Duration { get; set; }
}