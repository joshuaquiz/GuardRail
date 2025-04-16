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

namespace GuardRail.Local.Service.BackgroundServices;

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
            logger.LogInformation(
                "Checking for commands");
            var commands = await httpClient.GetFromJsonAsync<List<Command>>(
                $"/Command/ListPendingCommands?locationId={locationId}",
                stoppingToken);
            logger.LogInformation(
                $"Found {commands?.Count ?? 0} commands");
            foreach (var command in commands ?? [])
            {
                logger.LogInformation(
                    $"Processing command: {command.Guid} {command.Type} {command.Body}");
                await serviceProvider
                    .GetRequiredKeyedService<ICommandHandler>(
                        command.Type)
                    .ProcessCommand(
                        command,
                        stoppingToken);
                logger.LogInformation(
                    $"Completed command: {command.Guid}");
            }

            if (commands is { Count: > 0 })
            {
                logger.LogInformation(
                    "Completed all command");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(1),
                stoppingToken);
        }
    }
}