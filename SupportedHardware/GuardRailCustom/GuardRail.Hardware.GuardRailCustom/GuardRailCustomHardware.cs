using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.Common;
using GuardRail.Hardware.GuardRailCustom.BackgroundServices;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Logic.Commands.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class GuardRailCustomHardware(
    ILogger<GuardRailCustomHardware> logger,
    NetworkHardwareCache networkHardwareCache)
    : ISupportedHardware
{
    /// <inheritdoc />
    public static IServiceCollection Setup(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NetworkHardwareCache>();
        serviceCollection
            .AddSingleton(
                new HardwareDiscoveryPacket
                {
                    Port = new IPEndPoint(IPAddress.Loopback, 0).Port,
                    IpAddress = Dns.GetHostEntry(
                                        Dns.GetHostName())
                                    .AddressList
                                    .FirstOrDefault(x =>
                                        x.AddressFamily == AddressFamily.InterNetwork)
                                ?? IPAddress.Parse(
                                    "127.0.0.1"),
                    Name = Dns.GetHostName(),
                    IsRunningOnDevice = false
                });
        serviceCollection.AddSingleton<ISupportedHardware, GuardRailCustomHardware>();
        serviceCollection.AddHostedService<ServerHardwareUdpDiscoveryListenerBackgroundWorker>();
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

    /// <inheritdoc />
    public async Task UnlockDoor(
        UnlockDoorCommandData unlockDoorCommandData,
        CancellationToken cancellationToken)
    {
        var hardware = networkHardwareCache
            .Get(
                unlockDoorCommandData.HardwareId);
        if (hardware == null)
        {
            return;
        }

        try
        {
            await hardware.UdpClient
                .SendData(
                    GuardRailCustomConstants.UdpCommandNames.UnLockDoor,
                    unlockDoorCommandData,
                    cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogGuardRailError(e);
        }
    }
}