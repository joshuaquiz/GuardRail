using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.LocalClient.Data.Interfaces;

namespace GuardRail.LocalClient.Data
{
    internal sealed class DataStore : IDataStore
    {
        private readonly List<IDataSink> _dataSinks;

        internal DataStore(IEnumerable<IDataSink> dataSinks)
        {
            _dataSinks = dataSinks.ToList();
        }

        /// <inheritdoc />
        public void StartSync()
        {
            foreach (var dataSink in _dataSinks)
            {
                dataSink.StartSync();
            }
        }

        /// <inheritdoc />
        public async Task<T> SaveNew<T>(
            T item,
            CancellationToken cancellationToken)
            where T : class, IAddableItem
        {
            item.Guid = Guid.NewGuid();
            await Task.WhenAll(
                _dataSinks.Select(x => x.SaveNew(item, cancellationToken)));
            return item;
        }

        /// <inheritdoc />
        public async Task UpdateExisting<T>(
            T item,
            CancellationToken cancellationToken) =>
            await Task.WhenAll(
                _dataSinks.Select(x => x.UpdateExisting(item, cancellationToken)));

        /// <inheritdoc />
        public async Task DeleteExisting<T>(
            T item,
            CancellationToken cancellationToken) =>
            await Task.WhenAll(
                _dataSinks.Select(x => x.DeleteExisting(item, cancellationToken)));

        /// <inheritdoc />
        public async Task<IQueryable<T>> Get<T>(
            Expression<Func<IQueryable<T>>> getExpression,
            CancellationToken cancellationToken)
        {
            var resultQuery = await Task.WhenAny(
                _dataSinks.Select(x => x.Get(getExpression, cancellationToken)));
            return await resultQuery;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var dataSink in _dataSinks)
            {
                dataSink.Dispose();
            }
        }
    }
}