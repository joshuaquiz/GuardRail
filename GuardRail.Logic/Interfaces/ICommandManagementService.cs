using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for Command management.
/// </summary>
public interface ICommandManagementService
{
    /// <summary>
    /// Adds a new command into the system.
    /// </summary>
    /// <param name="locationId">The ID of the location to get the commands for.</param>
    /// <param name="status">The status of the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="Command"/> representing the work to get the commands.</returns>
    public Task<List<Command>> ListCommands(
        Guid locationId,
        CommandStatus? status,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new command into the system.
    /// </summary>
    /// <param name="locationId">The ID of the location this command is related to.</param>
    /// <param name="type">The command type.</param>
    /// <param name="expiryDate">The time the command expires.</param>
    /// <param name="maxRetries">The maximum number of times the command can be retried before being considered a failure.</param>
    /// <param name="body">The body of the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="Command"/> representing the work to add a new command.</returns>
    public Task<Command> AddNewCommand(
        Guid locationId,
        CommandType type,
        DateTimeOffset expiryDate,
        int? maxRetries,
        string body,
        CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new command into the system.
    /// </summary>
    /// <param name="commandId">The ID of the command.</param>
    /// <param name="status">The status of the command.</param>
    /// <param name="response">The body of the response to the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to update a command.</returns>
    public Task UpdateCommand(
        Guid commandId,
        CommandStatus status,
        string? response,
        CancellationToken cancellationToken);
}