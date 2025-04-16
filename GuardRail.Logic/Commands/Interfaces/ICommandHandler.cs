using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Commands.Interfaces;

/// <summary>
/// An interface defining a command handler.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Processes the command.
    /// </summary>
    /// <param name="command">The command to process.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to process the command.</returns>
    public Task ProcessCommand(
        Command command,
        CancellationToken cancellationToken);

    /// <summary>
    /// The command type associated with this handler.
    /// </summary>
    public CommandType CommandType { get; }
}