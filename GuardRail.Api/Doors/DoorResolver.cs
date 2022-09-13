using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Door = GuardRail.Api.Data.Door;

namespace GuardRail.Api.Doors;

/// <summary>
/// Manages the link between the device IDs and the hardware behind them.
/// </summary>
public sealed class DoorResolver : IDoorResolver
{
    private static readonly Dictionary<string, Door> Doors = new Dictionary<string, Door>();

    private readonly GuardRailContext _guardRailContext;

    public DoorResolver(
        GuardRailContext guardRailContext)
    {
        _guardRailContext = guardRailContext;
    }

    /// <summary>
    /// Adds a door to the resolver.
    /// </summary>
    /// <param name="door"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RegisterDoor(
        Door door,
        CancellationToken cancellationToken)
    {
        if (door == null)
        {
            throw new ArgumentNullException(nameof(door));
        }

        var doorId = await door.GetDeviceId();
        await _guardRailContext.Logs.AddAsync(
            new Log
            {
                DateTime = DateTimeOffset.UtcNow,
                LogMessage = $"Door {doorId} has been added"
            });
        Doors.Add(doorId, door);
    }

    public Task RegisterDoor(Core.DataModels.Door door, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<Core.DataModels.Door> IDoorResolver.GetDoorByDeviceId(string deviceId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns an <see cref="IDoor"/> of the hardware for an actual door.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Door> GetDoorByDeviceId(
        string deviceId,
        CancellationToken cancellationToken) =>
        Doors.ContainsKey(deviceId)
            ? Task.FromResult(Doors[deviceId])
            : Task.FromResult((Door)null);
}