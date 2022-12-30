using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// An account in the system.
/// </summary>
[Table("Accounts")]
public class Account : IAddableItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <summary>
    /// The name of the account.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The location of the installation.
    /// </summary>
    [Required]
    public string Location { get; set; } = string.Empty;
}