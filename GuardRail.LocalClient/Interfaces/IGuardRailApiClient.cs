using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.LocalClient.Data.Remote;

namespace GuardRail.LocalClient.Interfaces
{
    internal interface IGuardRailApiClient
    {
        Task UploadChangesAsync(
            IEnumerable<RemoteSinkAction> changes,
            CancellationToken cancellationToken);

        Task<IQueryable<T>> GetDataFromQuery<T>(
            Expression<Func<IQueryable<T>>> getExpression,
            CancellationToken cancellationToken);
    }
}