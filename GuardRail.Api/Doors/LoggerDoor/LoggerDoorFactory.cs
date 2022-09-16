using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;
using Door = GuardRail.Api.Data.Door;

namespace GuardRail.Api.Doors.LoggerDoor;

public sealed class LoggerDoorFactory : IDoorFactory
{
    private readonly GuardRailLogger _guardRailLogger;
    private readonly GuardRailContext _guardRailContext;

    public LoggerDoorFactory(
        GuardRailLogger guardRailLogger,
        GuardRailContext guardRailContext)
    {
        _guardRailLogger = guardRailLogger;
        _guardRailContext = guardRailContext;
    }

    public async Task<IReadOnlyCollection<Core.DataModels.Door>> GetDoors()
    {
        var doors = new ReadOnlyCollection<Door>(
            new List<Door>
            {
                new LoggerDoor("logger door", _guardRailLogger)
            });
        foreach (var door in doors)
        {
            var doorId = await door.GetDeviceId();
            var doorInDatabase = await _guardRailContext.Doors.SingleOrDefaultAsync(x => x.DeviceId == doorId);
            if (doorInDatabase == null)
            {
                await _guardRailContext.Doors.AddAsync(
                    new Door
                    {
                        DeviceId = doorId,
                        LockedStatus = await door.GetLockedStatus(CancellationToken.None),
                        FriendlyName = door.FriendlyName,
                        IsConfigured = false
                    });
                await _guardRailContext.SaveChangesAsync();
            }
            else
            {
                door.FriendlyName = doorInDatabase.FriendlyName;
            }
        }
        return doors;
    }
}