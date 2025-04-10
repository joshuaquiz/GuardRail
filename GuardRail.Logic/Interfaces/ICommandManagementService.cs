using System;
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
    /// <param name="type">The command type.</param>
    /// <param name="expiryDate">The time the command expires.</param>
    /// <param name="maxRetries">The maximum number of times the command can be retried before being considered a failure.</param>
    /// <param name="body">The body of the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="Command"/> representing the work to add a new command.</returns>
    public Task<Command> AddNewCommand(
        CommandType type,
        DateTimeOffset expiryDate,
        int? maxRetries,
        string body,
        CancellationToken cancellationToken);
}