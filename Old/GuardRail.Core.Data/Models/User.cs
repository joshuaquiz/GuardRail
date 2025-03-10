using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GuardRail.Core.Data.Interfaces;

namespace GuardRail.Core.Data.Models;

/// <summary>
/// A user in the system.
/// </summary>
public class User : IAddableItem, IAccountItem
{
    /// <inheritdoc />
    [Key]
    public Guid Guid { get; set; }

    /// <inheritdoc />
    [Required]
    public Guid AccountGuid { get; set; }

    /// <summary>
    /// The user's username.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's password.
    /// </summary>
    [Required]
    public byte[] Password { get; set; } = [];

    /// <summary>
    /// The next time the users password will expire.
    /// </summary>
    [Required]
    public DateOnly PasswordResetDate { get; set; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The user's last name.
    /// </summary>
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The user's phone.
    /// </summary>
    [Required]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The user's email.
    /// </summary>
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The Guid of the <see cref="UserGroup"/>, if there is one.
    /// </summary>
    public Guid? UserGroupGuid { get; set; }

    /// <summary>
    /// The <see cref="UserGroup"/>, if there is one.
    /// </summary>
    public UserGroup? UserGroup { get; set; }

    /// <summary>
    /// The <see cref="UserAccessMethod"/>s, linked to this user.
    /// </summary>
    public List<UserAccessMethod> UserAccessMethods { get; set; } = new();
}