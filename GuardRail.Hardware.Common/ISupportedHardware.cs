using GuardRail.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.Common;

public interface ISupportedHardware
{
    /// <summary>
    /// The type associated with this hardware.
    /// </summary>
    public AccessPointType AccessPointType { get; }

    /// <summary>
    /// Sets up the hardware for this provider.
    /// </summary>
    /// <param name="serviceCollection">A <see cref="IServiceCollection"/> to modify.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public IServiceCollection Setup(
        IServiceCollection serviceCollection);

    /// <summary>
    /// Lists all non-configured APs of this hardware type.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task{T}"/> of <see cref="IReadOnlyCollection{T}"/> of <see cref="string"/> representing the work to get the APs.</returns>
    public Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        CancellationToken cancellationToken);
}