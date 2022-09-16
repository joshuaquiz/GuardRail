using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces;
using Microsoft.Extensions.Hosting;

namespace GuardRail.DoorClient.Services;

public sealed class HardwareInitService : IHostedService
{
    private readonly IHardwareAsyncInit[] _hardwareAsyncInits;

    public HardwareInitService(
        IHardwareAsyncInit[] hardwareAsyncInits)
    {
        _hardwareAsyncInits = hardwareAsyncInits;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var asyncInit in _hardwareAsyncInits)
        {
            await asyncInit.InitAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}