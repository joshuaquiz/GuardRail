using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services;

public sealed class AsyncInitService : IHostedService
{
    private readonly IEnumerable<IAsyncInit> _hardwareAsyncInits;
    private readonly ILogger<AsyncInitService> _logger;

    public AsyncInitService(
        IEnumerable<IAsyncInit> hardwareAsyncInits,
        ILogger<AsyncInitService> logger)
    {
        _hardwareAsyncInits = hardwareAsyncInits;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.WhenAll(_hardwareAsyncInits.Select(async x => await x.InitAsync()));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}