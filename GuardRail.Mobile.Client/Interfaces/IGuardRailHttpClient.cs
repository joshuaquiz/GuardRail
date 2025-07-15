using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Mobile.Client.Interfaces;

public interface IGuardRailHttpClient
{
    public ValueTask<TResponse> GetData<TResponse>(
        string path,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry = null);

    public ValueTask<TResponse> GetSecureData<TResponse>(
        string path,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry = null);

    public ValueTask<TResponse> PostData<TResponse, TRequest>(
        string path,
        TRequest data,
        CancellationToken cancellationToken);

    public ValueTask<TResponse> PostSecureData<TResponse, TRequest>(
        string path,
        TRequest data,
        CancellationToken cancellationToken);
}