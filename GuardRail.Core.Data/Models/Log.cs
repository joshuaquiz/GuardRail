using System;
using GuardRail.Core.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// Represents a logged event.
/// </summary>
public class Log : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The timestamp of the log.
    /// </summary>
    [Required]
    public DateTimeOffset DateTime { get; set; }

    /// <summary>
    /// The message of the log.
    /// </summary>
    [Required]
    public string LogMessage { get; set; } = string.Empty;
}