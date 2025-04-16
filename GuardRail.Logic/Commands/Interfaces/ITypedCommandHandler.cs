using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Logic.Commands.Interfaces;

/// <summary>
/// An interface defining a typed command handler.
/// </summary>
public interface ITypedCommandHandler<in T> : ICommandHandler
{
    /// <summary>
    /// Processes the command.
    /// </summary>
    /// <param name="commandId">The ID of the command to process.</param>
    /// <param name="body">The body of the command to process.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to process the command.</returns>
    public Task ProcessCommandBody(
        Guid commandId,
        T? body,
        CancellationToken cancellationToken);
}