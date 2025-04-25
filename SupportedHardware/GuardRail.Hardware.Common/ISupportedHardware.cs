using GuardRail.Core.Enums;
using GuardRail.Logic.Commands.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.Common;

public interface ISupportedHardware
{
    /// <summary>
    /// Sets up the hardware for this provider.
    /// </summary>
    /// <param name="serviceCollection">A <see cref="IServiceCollection"/> to modify.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static virtual IServiceCollection Setup(
        IServiceCollection serviceCollection) => serviceCollection;

    /// <summary>
    /// The type associated with this hardware.
    /// </summary>
    public AccessPointType AccessPointType { get; }

    /// <summary>
    /// Lists all non-configured APs of this hardware type.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="string"/> representing the work to get the APs.</returns>
    public Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        CancellationToken cancellationToken);

    /// <summary>
    /// Unlocks a door.
    /// </summary>
    /// <param name="unlockDoorCommandData">Data needed to do the door unlock.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the work to unlock a door.</returns>
    Task UnlockDoor(
        UnlockDoorCommandData unlockDoorCommandData,
        CancellationToken cancellationToken);
}