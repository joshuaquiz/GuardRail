using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Data.Interfaces;
using GuardRail.LocalClient.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Data;

/// <inheritdoc />
public sealed class DataStore : IDataStore
{
    private readonly List<IDataSink> _dataSinks;

    /// <summary>
    /// A data store.
    /// </summary>
    public DataStore(IServiceProvider serviceProvider)
    {
        _dataSinks = serviceProvider.GetServices<IDataSink>().ToList();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
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
    public async Task<T> GetSingleOrDefault<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class
    {
        var resultQuery = await Task.WhenAny(
            _dataSinks.Select(x => x.GetSingleOrDefault(getExpression, cancellationToken)));
        return await resultQuery;
    }

    /// <inheritdoc />
    public async Task<T> GetFirstOrDefault<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class
    {
        var resultQuery = await Task.WhenAny(
            _dataSinks.Select(x => x.GetFirstOrDefault(getExpression, cancellationToken)));
        return await resultQuery;
    }

    /// <inheritdoc />
    public async Task<IQueryable<T>> GetData<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class
    {
        var resultQuery = await Task.WhenAny(
            _dataSinks.Select(x => x.GetData(getExpression, cancellationToken)));
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