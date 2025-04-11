using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Commands.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service;

public sealed class CommandProcessorBackgroundService(
    Guid locationId,
    HttpClient httpClient,
    IServiceProvider serviceProvider,
    ILogger<CommandProcessorBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var commands = await httpClient.GetFromJsonAsync<List<Command>>(
                $"/Command/ListPendingCommands?locationId={locationId}",
                stoppingToken);
            foreach (var command in commands ?? [])
            {
                await serviceProvider
                    .GetRequiredKeyedService<ICommandHandler>(
                        command.Type)
                    .ProcessCommand(
                        command,
                        stoppingToken);
            }
        }
    }
}