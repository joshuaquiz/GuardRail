using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.Api.Doors.LoggerDoor
{
    /// <summary>
    /// Manages the link between the device IDs and the hardware behind them.
    /// </summary>
    public sealed class DoorResolver : IDoorResolver
    {
        private static Dictionary<string, IDoor> Doors;

        /// <summary>
        /// Adds a door to the resolver.
        /// </summary>
        /// <param name="door"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RegisterDoor(IDoor door, CancellationToken cancellationToken)
        {
            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            Doors.Add(await door.GetDeviceId(), door);
        }

        /// <summary>
        /// Returns an <see cref="IDoor"/> of the hardware for an actual door.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IDoor> GetDoorByDeviceId(string deviceId, CancellationToken cancellationToken) =>
            Doors.ContainsKey(deviceId)
                ? Task.FromResult(Doors[deviceId])
                : Task.FromResult((IDoor)null);
    }
}