using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service;

public sealed class DataSyncWorker(
    HttpClient httpClient,
    ILogger<DataSyncWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // TODO: Data sync.
        }
    }
}