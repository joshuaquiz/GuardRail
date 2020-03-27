using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;

namespace GuardRail.Api.Doors
{
    /// <summary>
    /// Manages the link between the device IDs and the hardware behind them.
    /// </summary>
    public sealed class DoorResolver : IDoorResolver
    {
        private static readonly Dictionary<string, IDoor> Doors = new Dictionary<string, IDoor>();

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
            IDoor door,
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

        /// <summary>
        /// Returns an <see cref="IDoor"/> of the hardware for an actual door.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IDoor> GetDoorByDeviceId(
            string deviceId,
            CancellationToken cancellationToken) =>
            Doors.ContainsKey(deviceId)
                ? Task.FromResult(Doors[deviceId])
                : Task.FromResult((IDoor)null);
    }
}