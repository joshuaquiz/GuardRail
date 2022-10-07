using System.Net.Http.Json;
using GuardRail.DeviceLogic.Interfaces.Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.DeviceLogic.Implementations.Communication;

public class CoreCentralServerCommunication<T> : ICentralServerCommunication where T : CoreCentralServerCommunication<T>
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger<T> Logger;

    public CoreCentralServerCommunication(
        IServiceProvider serviceProvider,
        ILogger<T> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

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
                path,
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
                path,
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