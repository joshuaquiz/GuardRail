using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service.BackgroundServices;

public sealed class AutoUpdateCheckerBackgroundService(
    HttpClient httpClient,
    ILogger<AutoUpdateCheckerBackgroundService> logger)
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
                $"/Version/VersionCheck?version={_currentVersion}",
                stoppingToken);
            if (response?.IsLatest == true)
            {
                try
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
                catch (OperationCanceledException)
                {
                    continue;
                }
            }

            // TODO: Trigger update.
        }
    }
}