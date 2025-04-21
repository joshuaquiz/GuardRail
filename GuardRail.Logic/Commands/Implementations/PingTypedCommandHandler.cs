using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;

namespace GuardRail.Logic.Commands.Implementations;

/// <summary>
/// Handles the <see cref="CommandType.Ping"/> command type.
/// </summary>
public sealed class PingTypedCommandHandler(
    HttpClient httpClient)
    : TypedCommandHandlerBase<DateTimeOffset>
{
    /// <inheritdoc />
    public override async Task ProcessCommandBody(
        Guid commandId,
        DateTimeOffset body,
        CancellationToken cancellationToken)
    {
        await httpClient.PostAsync(
            $"/Command/UpdateCommand?commandId={commandId}&status={CommandStatus.CompletedSuccessfully}",
            new StringContent(DateTimeOffset.UtcNow.ToString("O")),
            cancellationToken);
    }

    /// <summary>
    /// The command type associated with this handler.
    /// </summary>
    public static CommandType CommandType =>
        CommandType.Ping;
}