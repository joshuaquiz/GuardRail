using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.LocalClient.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Data
{
    internal sealed class DataStore : IDataStore
    {
        private readonly IServiceProvider _serviceProvider;
        private List<IDataSink> _dataSinks;

        internal DataStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dataSinks = _serviceProvider.GetServices<IDataSink>().ToList();
            foreach (var dataSink in _dataSinks)
            {
                dataSink.StartSync();
            }
            
            return Task.CompletedTask;
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
        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var dataSink in _dataSinks)
            {
                dataSink.Dispose();
            }
            
            return Task.CompletedTask;
        }
    }
}