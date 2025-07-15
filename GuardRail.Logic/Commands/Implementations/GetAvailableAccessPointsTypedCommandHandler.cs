using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.Common;

namespace GuardRail.Logic.Commands.Implementations;

/// <summary>
/// Handles the <see cref="CommandType.GetAvailableAccessPoints"/> command type.
/// </summary>
public sealed class GetAvailableAccessPointsTypedCommandHandler(
    IEnumerable<ISupportedHardware> supportedHardware,
    HttpClient httpClient)
    : TypedCommandHandlerBase<AccessPointType>
{
    /// <inheritdoc />
    public override async Task ProcessCommandBody(
        Guid commandId,
        AccessPointType body,
        CancellationToken cancellationToken)
    {
        var hardware = supportedHardware.FirstOrDefault(x => x.AccessPointType == body);
        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.InProgress}",
            new StringContent(
                "LOADING"),
            cancellationToken);
        var availableAccessPoints = await hardware
            !.GetAvailableAccessPoints(
                cancellationToken);
        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.CompletedSuccessfully}",
            new StringContent(
                availableAccessPoints
                    .ToJson()),
            cancellationToken);
    }

    /// <summary>
    /// The command type associated with this handler.
    /// </summary>
    public static CommandType CommandType =>
        CommandType.GetAvailableAccessPoints;
}