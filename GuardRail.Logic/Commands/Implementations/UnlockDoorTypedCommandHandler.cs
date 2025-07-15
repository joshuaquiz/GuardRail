using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models;
using GuardRail.Hardware.Common;

namespace GuardRail.Logic.Commands.Implementations;

/// <summary>
/// Handles the <see cref="CommandType.UnlockDoor"/> command type.
/// </summary>
public sealed class UnlockDoorTypedCommandHandler(
    IEnumerable<ISupportedHardware> supportedHardware,
    HttpClient httpClient)
    : TypedCommandHandlerBase<UnlockDoorCommandData>
{
    /// <inheritdoc />
    public override async Task ProcessCommandBody(
        Guid commandId,
        UnlockDoorCommandData? body,
        CancellationToken cancellationToken)
    {
        if (body == null)
        {
            return;
        }

        var matchedHardware = supportedHardware
            .FirstOrDefault(
                x =>
                    x.AccessPointType == body.AccessPointType);
        if (matchedHardware == null)
        {
            return;
        }

        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.InProgress}",
            new StringContent(
                "LOADING"),
            cancellationToken);
        await matchedHardware.UnlockDoor(
            body,
            cancellationToken);
        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.CompletedSuccessfully}",
            new StringContent(
                $"The door {body.HardwareId} was unlocked"),
            cancellationToken);
    }

    /// <summary>
    /// The command type associated with this handler.
    /// </summary>
    public static CommandType CommandType =>
        CommandType.UnlockDoor;
}