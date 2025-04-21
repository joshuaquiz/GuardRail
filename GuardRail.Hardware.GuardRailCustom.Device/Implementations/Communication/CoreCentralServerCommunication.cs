using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Communication;

public class CoreCentralServerCommunication<T>(
    IServiceProvider serviceProvider,
    ILogger<T> logger) : ICentralServerCommunication
    where T : CoreCentralServerCommunication<T>
{
    protected readonly IServiceProvider ServiceProvider = serviceProvider;
    protected readonly ILogger<T> Logger = logger;

    /// <inheritdoc />
    public async ValueTask<bool> SendDataAsync<TData>(
        string path,
        TData data,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            await httpClient.PostAsJsonAsync(
                "api/device_actions/" + path,
                data,
                cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask<Stream> GetDataAsync(
        string path,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            return await httpClient.GetStreamAsync(
                "api/device_actions/" + path,
                cancellationToken);
        }
        catch (Exception)
        {
            return new MemoryStream();
        }
    }

    /// <inheritdoc />
    public void Dispose() =>
        DisposeAsync().GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;
}