using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// A user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the account.
    /// </summary>
    public required Guid AccountGuid { get; set; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// The user's phone.
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// The user's email.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// The next time the users password will expire.
    /// </summary>
    public required DateOnly PasswordResetDate { get; set; }

    /// <summary>
    /// The type of the user.
    /// </summary>
    public required UserType UserType { get; set; }
}