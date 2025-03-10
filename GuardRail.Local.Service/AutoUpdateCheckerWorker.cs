using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service;

public sealed class AutoUpdateCheckerWorker(
    HttpClient httpClient,
    ILogger<AutoUpdateCheckerWorker> logger)
    : BackgroundService
{
    private readonly Version? _currentVersion = Assembly.GetCallingAssembly()
        .GetName()
        .Version;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                $"Checking the status of version '{_currentVersion}'");
            var response = await httpClient.GetFromJsonAsync<VersionCheckResponse>(
                $"/Version?version={_currentVersion}",
                stoppingToken);
            if (response?.IsLatest == true)
            {
                await Task.Delay(
                    TimeSpan.FromDays(
                        1)
                    + TimeSpan.FromSeconds(
                        Random.Shared.Next(
                            -100,
                            100)),
                    stoppingToken);
                continue;
            }

            // TODO: Trigger update.
        }
    }
}