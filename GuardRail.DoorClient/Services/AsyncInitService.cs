using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces;
using Microsoft.Extensions.Hosting;

namespace GuardRail.DoorClient.Services;

public sealed class AsyncInitService : IHostedService
{
    private readonly IAsyncInit[] _hardwareAsyncInits;

    public AsyncInitService(
        IAsyncInit[] hardwareAsyncInits)
    {
        _hardwareAsyncInits = hardwareAsyncInits;
    }

    public Task StartAsync(CancellationToken cancellationToken) =>
        Task.WhenAll(_hardwareAsyncInits.Select(async x => await x.InitAsync()));

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}