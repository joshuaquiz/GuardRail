using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Data.Models;
using GuardRail.LocalClient.Data.Remote;

namespace GuardRail.LocalClient.Interfaces;

/// <summary>
/// A wrapper for the API.
/// </summary>
public interface IGuardRailApiClient
{
    /// <summary>
    /// Upload data changes for syncing.
    /// </summary>
    /// <param name="changes">The changes to sync.</param>
    /// <param name="cancellationToken">A CancellationToken.</param>
    /// <returns>Task</returns>
    Task UploadChangesAsync(
        IEnumerable<RemoteSinkAction> changes,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get data from the remote data store.
    /// </summary>
    /// <typeparam name="T">The type of data to get.</typeparam>
    /// <param name="getExpression">The expression to use to pull data.</param>
    /// <param name="cancellationToken">A CancellationToken.</param>
    /// <returns>Task of IQueryable of T</returns>
    Task<IQueryable<T>> GetDataFromQuery<T>(
        Expression<Func<IQueryable<T>>> getExpression,
        CancellationToken cancellationToken);

    /// <summary>
    /// Create a new account.
    /// </summary>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="location">The location of the account.</param>
    /// <param name="firstName">The first name of the main user.</param>
    /// <param name="lastName">The last name of the main user.</param>
    /// <param name="phone">The phone number of the main user.</param>
    /// <param name="email">The email of the main user.</param>
    /// <returns>Task of Account</returns>
    Task<Account> CreateNewAccount(
        string accountName,
        string location,
        string firstName,
        string lastName,
        string phone,
        string email);
}