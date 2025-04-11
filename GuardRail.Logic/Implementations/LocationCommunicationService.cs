using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Exceptions;
using GuardRail.Core.Helpers;
using GuardRail.Logic.Interfaces;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// Handles calls to a local system.
/// </summary>
public sealed class LocationCommunicationService(
    ICommandManagementService commandManagementService)
    : ILocationCommunicationService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        Guid locationId,
        AccessPointType accessPointType,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var command = await commandManagementService
            .AddNewCommand(
                locationId,
                CommandType.GetAvailableAccessPoints,
                DateTimeOffset.UtcNow.Add(timeout),
                0,
                accessPointType.ToString("G"),
                cancellationToken);
        return await GetCommandResponse<List<string>>(
            command.Guid,
            timeout,
            cancellationToken);
    }

    private async Task<T> GetCommandResponse<T>(
        Guid commandId,
        TimeSpan timeout,
        CancellationToken cancellationToken)
        where T : class
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);
        while (!cts.IsCancellationRequested)
        {
            var command = await commandManagementService.GetCommand(
                commandId,
                cts.Token);
            switch (command!.Status)
            {
                case CommandStatus.Pending:
                case CommandStatus.InProgress:
                    await Task.Delay(
                        TimeSpan.FromSeconds(1),
                        cts.Token);
                    break;
                case CommandStatus.CompletedSuccessfully:
                case CommandStatus.CompletedWithErrors:
                {
                    try
                    {
                        return command.Response
                                      ?.FromJson<T>()
                                  ?? throw new Exception(
                                      "Failed to parse response");
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCommandResponseFormatException(
                            command.Response,
                            e);
                    }
                }
                case CommandStatus.Failed:
                {
                    var commandResponse = command.Response
                        ?.Split(
                            Environment.NewLine)
                        ?? [ "EMPTY RESPONSE" ];
                    throw new CommandFailureException(
                        command.Guid,
                        commandResponse[0],
                        string.Join(
                            Environment.NewLine,
                            commandResponse
                                .Skip(
                                    1)));
                }
                case CommandStatus.Expired:
                {
                    throw new CommandFailureException(
                        command.Guid,
                        "The local system did not respond in time.");
                }
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(command.Status),
                        command.Status,
                        "Unknown command status");
            }
        }

        throw new CommandFailureException(
            commandId,
            "The local system did not respond in time.",
            "Command listener timed out");
    }
}