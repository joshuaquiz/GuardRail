using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Data;
using GuardRail.LocalClient.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Local;

/// <summary>
/// An EntityFrameWork IDataSink implementation.
/// </summary>
public sealed class EntityFrameWorkDataSink : IDataSink
{
    private readonly GuardRailContext _guardRailContext;

    /// <summary>
    /// Creates an EntityFrameWorkDataSink.
    /// </summary>
    public EntityFrameWorkDataSink(GuardRailContext guardRailContext)
    {
        _guardRailContext = guardRailContext;
    }

    /// <inheritdoc />
    public void StartSync()
    {

    }

    /// <inheritdoc />
    public async Task<T> SaveNew<T>(
        T item,
        CancellationToken cancellationToken) where T : class
    {
        var result = await _guardRailContext.AddAsync(item, cancellationToken);
        await _guardRailContext.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task UpdateExisting<T>(
        T item,
        CancellationToken cancellationToken)
    {
        _guardRailContext.Update(item);
        await _guardRailContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteExisting<T>(
        T item,
        CancellationToken cancellationToken)
    {
        _guardRailContext.Remove(item);
        await _guardRailContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T> GetSingleOrDefault<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class =>
        await _guardRailContext.Set<T>().SingleOrDefaultAsync(getExpression, cancellationToken);

    /// <inheritdoc />
    public async Task<T> GetFirstOrDefault<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class =>
        await _guardRailContext.Set<T>().FirstOrDefaultAsync(getExpression, cancellationToken);

    /// <inheritdoc />
    public Task<IQueryable<T>> GetData<T>(
        Expression<Func<T, bool>> getExpression,
        CancellationToken cancellationToken)
        where T : class =>
        Task.FromResult(_guardRailContext.Set<T>().Where(getExpression));

    /// <inheritdoc />
    public void Dispose()
    {
        _guardRailContext?.Dispose();
    }
}