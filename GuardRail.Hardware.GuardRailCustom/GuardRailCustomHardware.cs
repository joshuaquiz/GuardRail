using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Hardware.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class GuardRailCustomHardware(
    NetworkHardwareCache networkHardwareCache) : ISupportedHardware
{
    /// <inheritdoc />
    public static IServiceCollection Setup(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NetworkHardwareCache>();
        serviceCollection.AddSingleton<ISupportedHardware, GuardRailCustomHardware>();
        serviceCollection.AddHostedService<HardwareUdpDiscoveryBackgroundWorker>();
        return serviceCollection;
    }

    /// <inheritdoc />
    public AccessPointType AccessPointType =>
        AccessPointType.GuardRailCustom;

    /// <inheritdoc />
    public Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<string>>(
            networkHardwareCache
                .GetAll()
                .Select(
                    x =>
                        x.Name)
                .ToList());
}