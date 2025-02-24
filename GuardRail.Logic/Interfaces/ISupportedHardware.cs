using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;

namespace GuardRail.Logic.Interfaces;

public interface ISupportedHardware
{
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
}