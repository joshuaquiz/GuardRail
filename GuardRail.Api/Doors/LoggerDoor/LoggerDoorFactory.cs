using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GuardRail.Api.Doors.LoggerDoor
{
    public sealed class LoggerDoorFactory : IDoorFactory
    {
        private readonly ILogger _logger;
        private readonly GuardRailContext _guardRailContext;

        public LoggerDoorFactory(
            ILogger logger,
            GuardRailContext guardRailContext)
        {
            _logger = logger;
            _guardRailContext = guardRailContext;
        }

        public async Task<IReadOnlyCollection<IDoor>> GetDoors()
        {
            var doors = new ReadOnlyCollection<IDoor>(
                new List<IDoor>
                {
                    new LoggerDoor(_logger)
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
                            LockedStatus = await door.GetLockedStatus(CancellationToken.None)
                        });
                    await _guardRailContext.SaveChangesAsync();
                }
            }
            return doors;
        }
    }
}