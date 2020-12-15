using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Interfaces;

namespace GuardRail.LocalClient.Data.Local
{
    /// <summary>
    /// An EntityFrameWork IDataSink implementation.
    /// </summary>
    internal sealed class EntityFrameWorkDataSink : IDataSink
    {
        private readonly GuardRailContext _guardRailContext;
        private readonly GuardRailBackgroundWorker _guardRailBackgroundWorker;

        /// <summary>
        /// Creates an EntityFrameWorkDataSink.
        /// </summary>
        internal EntityFrameWorkDataSink(GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
            _guardRailBackgroundWorker = GuardRailBackgroundWorker.Create(
                "EF Sink Save Changes",
                TimeSpan.FromMilliseconds(500),
                ct => _guardRailContext.SaveChangesAsync(ct),
                CancellationToken.None);
        }

        /// <inheritdoc />
        public void StartSync() =>
            _guardRailBackgroundWorker.Start();

        /// <inheritdoc />
        public async Task<T> SaveNew<T>(
            T item,
            CancellationToken cancellationToken) where T : class
        {
            var result = await _guardRailContext.AddAsync(item, cancellationToken);
            return result.Entity;
        }

        /// <inheritdoc />
        public Task UpdateExisting<T>(
            T item,
            CancellationToken cancellationToken)
        {
            _guardRailContext.Update(item);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteExisting<T>(
            T item,
            CancellationToken cancellationToken)
        {
            _guardRailContext.Remove(item);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IQueryable<T>> Get<T>(
            Expression<Func<IQueryable<T>>> getExpression,
            CancellationToken cancellationToken) =>
            Task.FromResult(_guardRailContext.FromExpression(getExpression));

        /// <inheritdoc />
        public void Dispose()
        {
            _guardRailBackgroundWorker.Dispose();
            _guardRailContext?.Dispose();
        }
    }
}