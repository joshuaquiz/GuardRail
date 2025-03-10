using System;
using GuardRail.Mobile.Client.Interfaces;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Mobile.Client.Exceptions;
using Microsoft.Maui.Networking;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Maui.Devices;

namespace GuardRail.Mobile.Client.Implementations;

public sealed class GuardRailHttpClient(
        IConnectivity connectivity,
        HttpClient httpClient,
        IMemoryCache memoryCache,
        IGuardRailStorage storage,
        IWiFi wifi)
    : IGuardRailHttpClient
{
    private readonly SemaphoreSlim _lookupSemaphore = new(1);
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<HttpMethod, SemaphoreSlim>> _semaphoreSlims = new();

    public async ValueTask<TResponse> GetData<TResponse>(
        string path,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry = null) =>
        await GetDataInternal<TResponse>(
            path,
            cancellationToken,
            cacheExpiry,
            false);

    public async ValueTask<TResponse> GetSecureData<TResponse>(
        string path,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry = null) =>
        await GetDataInternal<TResponse>(
            path,
            cancellationToken,
            cacheExpiry,
            true);

    public async ValueTask<TResponse> PostData<TResponse, TRequest>(
        string path,
        TRequest data,
        CancellationToken cancellationToken) =>
        await PostDataInternal<TResponse, TRequest>(
            path,
            data,
            false,
            cancellationToken);

    public async ValueTask<TResponse> PostSecureData<TResponse, TRequest>(
        string path,
        TRequest data,
        CancellationToken cancellationToken) =>
        await PostDataInternal<TResponse, TRequest>(
            path,
            data,
            true,
            cancellationToken);

    private async ValueTask<TResponse> PostDataInternal<TResponse, TRequest>(
        string path,
        TRequest data,
        bool securityCheck,
        CancellationToken cancellationToken)
    {
        var url = GetUrl(
            path,
            securityCheck);
        await SemaphoreSetup(
            HttpMethod.Post,
            path,
            cancellationToken);
        await _semaphoreSlims[path][HttpMethod.Post].WaitAsync(
            cancellationToken);
        try
        {
            var result = await httpClient.PostAsJsonAsync(
                url,
                data,
                cancellationToken);
            var resultModel = await result.Content.ReadFromJsonAsync<TResponse>(
                cancellationToken: cancellationToken);
            if (resultModel == null)
            {
                throw new Exception(
                    "No data returned");
            }
            else
            {
                memoryCache.Remove(
                    path);
                return resultModel;
            }
        }
        finally
        {
            _semaphoreSlims[path][HttpMethod.Post].Release(
                1);
        }
    }

    private async ValueTask<TResponse> GetDataInternal<TResponse>(
        string path,
        CancellationToken cancellationToken,
        TimeSpan? cacheExpiry,
        bool securityCheck)
    {
        await SemaphoreSetup(
            HttpMethod.Get,
            path,
            cancellationToken);
        await _semaphoreSlims[path][HttpMethod.Get].WaitAsync(
            cancellationToken);
        try
        {
            return await memoryCache.GetOrCreateAsync(
                       path,
                       async item =>
                       {
                           try
                           {
                               var url = GetUrl(
                                   path,
                                   securityCheck);
                               var result = await httpClient.GetFromJsonAsync<TResponse>(
                                                url,
                                                cancellationToken)
                                            ?? throw new Exception(
                                                "No data returned");
                               item.Value = result;
                               item.AbsoluteExpirationRelativeToNow = cacheExpiry ?? TimeSpan.FromSeconds(3);
                               return result;
                           }
                           catch
                           {
                               item.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                               throw;
                           }
                       })
                   ?? throw new Exception(
                       "No data returned");
        }
        finally
        {
            _semaphoreSlims[path][HttpMethod.Get].Release(
                1);
        }
    }

    private string GetUrl(
        string path,
        bool securityCheck)
    {
        var user = storage.GetUser();
        if ((user == null || (securityCheck && !user.UseLocalNetwork))
            && connectivity.NetworkAccess
                is NetworkAccess.Internet
                or NetworkAccess.ConstrainedInternet)
        {
#if DEBUG
            return DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7026" + path
                : "https://localhost:7026" + path;
#else
            return "https://api.example.com" + path;
#endif
        }

        if (!securityCheck
            || user == null)
        {
            throw new NoInternetException();
        }

        if (user.WiFiName == wifi.GetSsid())
        {
            return user.RootAddress + path;
        }

        var connected = wifi.TryConnectToNetwork(
            user.WiFiName!,
            Guid.NewGuid().ToString());
        return connected
            ? user.RootAddress + path
            : throw new NoInternetException();

    }

    private async Task SemaphoreSetup(
        HttpMethod httpMethod,
        string path,
        CancellationToken cancellationToken)
    {
        await _lookupSemaphore.WaitAsync(
            cancellationToken);
        if (!_semaphoreSlims.ContainsKey(
                path))
        {
            _semaphoreSlims.TryAdd(
                path,
                new ConcurrentDictionary<HttpMethod, SemaphoreSlim>());
        }

        if (!_semaphoreSlims[path].ContainsKey(
                httpMethod))
        {
            _semaphoreSlims[path].TryAdd(
                httpMethod,
                new SemaphoreSlim(1));
        }

        _lookupSemaphore.Release(
            1);
    }
}