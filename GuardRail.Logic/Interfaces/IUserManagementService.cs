using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Exceptions;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for user management.
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// Attempts to sign a user in and returns a new auth token, or specific exceptions.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="Guid"/> representing the work to log in and get a new auth token.</returns>
    /// <exception cref="InvalidEmailPasswordException">This exception is thrown when an invalid username and password are used.</exception>
    public Task<Guid> SignIn(
        string email,
        string password,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="firstName">The first name of the new user.</param>
    /// <param name="lastName">The last name of the new user.</param>
    /// <param name="phone">The user's phone.</param>
    /// <param name="email">The user's email.</param>
    /// <param name="userType">The user's type.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="User"/> representing the work to create the new user.</returns>
    public Task<User> CreateNewUser(
        Guid accountId,
        string firstName,
        string lastName,
        string phone,
        string email,
        UserType userType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Resets the password for a user.
    /// </summary>
    /// <param name="userId">The ID of the user whose password we are resetting.</param>
    /// <param name="newPassword"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to reset the password.</returns>
    /// <exception cref="UserCouldNotBeFoundException">Returned if the user ID is invalid.</exception>
    public Task ResetPassword(
        Guid userId,
        string newPassword,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets all the users for an account.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="tags">Any tags to use for filtering.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="User"/> representing the work to get the list of users.</returns>
    public Task<IReadOnlyCollection<User>> ListUsers(
        Guid accountId,
        IReadOnlyCollection<string>? tags,
        CancellationToken cancellationToken);
}