using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Commands.Interfaces;

namespace GuardRail.Logic.Commands.Implementations;

/// <summary>
/// Base handler for commands
/// </summary>
public abstract class TypedCommandHandlerBase<T>
    : ITypedCommandHandler<T>
{
    /// <inheritdoc />
    public async Task ProcessCommand(
        Command command,
        CancellationToken cancellationToken) =>
        await ProcessCommandBody(
            command.Guid,
            command.Body.FromJson<T>(),
            cancellationToken);

    /// <inheritdoc />
    public abstract Task ProcessCommandBody(
        Guid commandId,
        T? body,
        CancellationToken cancellationToken);
}