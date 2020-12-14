using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Local;

namespace GuardRail.LocalClient.Data
{
    /// <summary>
    /// An EntityFrameWork IDataSink implementation. 
    /// </summary>
    public sealed class EntityFrameWorkDataSink : IDataSink
    {
        private readonly GuardRailContext _guardRailContext;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly GuardRailBackgroundWorker _guardRailBackgroundWorker;

        /// <summary>
        /// Creates an EntityFrameWorkDataSink.
        /// </summary>
        public EntityFrameWorkDataSink()
        {
            _guardRailContext = new GuardRailContext();
            _cancellationTokenSource = new CancellationTokenSource();
            _guardRailBackgroundWorker = GuardRailBackgroundWorker.Create(
                "EF Sick",
                TimeSpan.FromMilliseconds(500),
                ct => _guardRailContext.SaveChangesAsync(ct),
                CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<T> SaveNew<T>(T item) where T : class
        {
            var result = await _guardRailContext.AddAsync(item, _cancellationTokenSource.Token);
            return result.Entity;
        }

        /// <inheritdoc />
        public Task UpdateExisting<T>(T item) =>
            Task.FromResult(_guardRailContext.Update(item).Entity);

        /// <inheritdoc />
        public Task DeleteExisting<T>(T item) =>
            Task.FromResult(_guardRailContext.Remove(item).Entity);

        /// <inheritdoc />
        public Task<IQueryable<T>> Get<T>(Expression<Func<IQueryable<T>>> getExpression) =>
            Task.FromResult(_guardRailContext.FromExpression(getExpression));

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _guardRailBackgroundWorker.Dispose();
            _guardRailContext?.Dispose();
        }
    }
}