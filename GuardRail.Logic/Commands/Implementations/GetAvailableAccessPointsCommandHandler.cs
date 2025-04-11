using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;

namespace GuardRail.Logic.Commands.Implementations;

/// <summary>
/// Handles the <see cref="CommandType.GetAvailableAccessPoints"/> command type.
/// </summary>
public sealed class GetAvailableAccessPointsCommandHandler(
    HttpClient httpClient)
    : CommandHandlerBase<AccessPointType>
{
    /// <inheritdoc />
    public override async Task ProcessCommandBody(
        Guid commandId,
        AccessPointType body,
        CancellationToken cancellationToken)
    {
        // TODO: Implement the logic to get available access points.

        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.CompletedSuccessfully}",
            new StringContent(DateTimeOffset.UtcNow.ToString("O")),
            cancellationToken);
    }
}