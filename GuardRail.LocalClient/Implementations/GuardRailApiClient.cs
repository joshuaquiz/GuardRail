using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Data.Models;
using GuardRail.LocalClient.Data.Remote;
using GuardRail.LocalClient.Interfaces;

namespace GuardRail.LocalClient.Implementations;

/// <inheritdoc />
internal sealed class GuardRailApiClient : IGuardRailApiClient
{
    /// <inheritdoc />
    public Task UploadChangesAsync(IEnumerable<RemoteSinkAction> changes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IQueryable<T>> GetDataFromQuery<T>(Expression<Func<IQueryable<T>>> getExpression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Account> CreateNewAccount(string accountName, string location, string firstName, string lastName, string phone,
        string email)
    {
        throw new NotImplementedException();
    }
}