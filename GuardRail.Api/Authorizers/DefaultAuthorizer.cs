using System;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Authorizers
{
    /// <summary>
    /// Everyone gets in all the time!
    /// </summary>
    public sealed class DefaultAuthorizer : IAuthorizer
    {
        private readonly GuardRailContext _guardRailContext;
        private readonly GuardRailLogger _guardRailLogger;

        public DefaultAuthorizer(
            GuardRailContext guardRailContext,
            GuardRailLogger guardRailLogger)
        {
            _guardRailContext = guardRailContext;
            _guardRailLogger = guardRailLogger;
        }

        /// <summary>
        /// Is device allowed? Yes!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessControlDevice"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceAuthorizedAtLocation(
            IUser user,
            IAccessControlDevice accessControlDevice)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    nameof(user));
            }

            if (accessControlDevice == null)
            {
                throw new ArgumentNullException(
                    nameof(accessControlDevice));
            }

            var accessControlDeviceId = await accessControlDevice.GetDeviceId();
            var dbData = await _guardRailContext
                .AccessControlDevices
                .Select(a =>
                    new
                    {
                        AccessControlDevice = a,
                        Users = a.UserAccesses
                            .Select(u => u.User)
                            .Concat(
                                a.AccessControlDeviceRoles.SelectMany(u => u.Role.Users))
                            .GroupBy(x => x.Id)
                            .Select(x => x.First())
                    })
                .SingleAsync(x => x.AccessControlDevice.DeviceId == accessControlDeviceId);
            var userFromDatabase = await _guardRailContext.Users.SingleAsync(x => x.Id == user.Id);
            if (dbData.Users.Contains(userFromDatabase))
            {
                await _guardRailLogger.LogAsync($"User {user.FirstName} {user.LastName} is granted access using {accessControlDeviceId}");
                return true;
            }
            else
            {
                await _guardRailLogger.LogAsync($"User {user.FirstName} {user.LastName} is NOT granted access using {accessControlDeviceId}");
                return false;
            }
        }
    }
}