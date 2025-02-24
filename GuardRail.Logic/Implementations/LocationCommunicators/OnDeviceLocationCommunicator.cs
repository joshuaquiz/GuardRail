using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Interfaces;

namespace GuardRail.Logic.Implementations.LocationCommunicators;

/// <summary>
/// A communicator used for on-device communication.
/// </summary>
public sealed class OnDeviceLocationCommunicator : ILocationCommunicator
{
    private readonly IReadOnlyDictionary<AccessPointType, ISupportedHardware> _supportedHardware;

    /// <summary>
    /// A communicator used for on-device communication.
    /// </summary>
    public OnDeviceLocationCommunicator(
        IEnumerable<ISupportedHardware> supportedHardware)
    {
        _supportedHardware = supportedHardware
            .ToDictionary(
                x =>
                    x.AccessPointType,
                x =>
                    x);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        AccessPointType accessPointType,
        CancellationToken cancellationToken) =>
        await _supportedHardware[accessPointType]
            .GetAvailableAccessPoints(cancellationToken);
}