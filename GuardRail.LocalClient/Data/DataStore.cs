using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GuardRail.LocalClient.Data
{
    public interface IAddableItem
    {
        Guid Guid { get; set; }
    }
    public interface IDataStore : IDisposable
    {
        Task<T> SaveNew<T>(T item)
            where T : class, IAddableItem;

        Task UpdateExisting<T>(T item);

        Task DeleteExisting<T>(T item);

        Task<IQueryable<T>> Get<T>(Expression<Func<IQueryable<T>>> getExpression);
    }

    public sealed class DataStore : IDataStore
    {
        private readonly List<IDataSink> _dataSinks;

        public DataStore(List<IDataSink> dataSinks)
        {
            _dataSinks = dataSinks;
        }

        /// <inheritdoc />
        public async Task<T> SaveNew<T>(T item)
            where T : class, IAddableItem
        {
            item.Guid = Guid.NewGuid();
            await Task.WhenAll(
                _dataSinks.Select(x => x.SaveNew(item)));
            return item;
        }

        /// <inheritdoc />
        public async Task UpdateExisting<T>(T item) =>
            await Task.WhenAll(
                _dataSinks.Select(x => x.UpdateExisting(item)));

        /// <inheritdoc />
        public async Task DeleteExisting<T>(T item) =>
            await Task.WhenAll(
                _dataSinks.Select(x => x.DeleteExisting(item)));

        /// <inheritdoc />
        public async Task<IQueryable<T>> Get<T>(Expression<Func<IQueryable<T>>> getExpression)
        {
            var resultQuery = await Task.WhenAny(
                _dataSinks.Select(x => x.Get(getExpression)));
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

    public interface IDataSink : IDisposable
    {
        Task<T> SaveNew<T>(T item)
            where T : class;

        Task UpdateExisting<T>(T item);

        Task DeleteExisting<T>(T item);

        Task<IQueryable<T>> Get<T>(Expression<Func<IQueryable<T>>> getExpression);
    }
}