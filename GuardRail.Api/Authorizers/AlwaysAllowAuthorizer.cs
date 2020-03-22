using System;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;

namespace GuardRail.Api.Authorizers
{
    /// <summary>
    /// Everyone gets in all the time!
    /// </summary>
    public sealed class AlwaysAllowAuthorizer : IAuthorizer
    {
        private readonly GuardRailContext _guardRailContext;

        public AlwaysAllowAuthorizer(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
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
            await _guardRailContext.Logs.AddAsync(
                new Log
                {
                    DateTime = DateTimeOffset.UtcNow,
                    LogMessage = $"User {user.FirstName} {user.LastName} is granted access using {accessControlDevice.GetDeviceId()}"
                });
            await _guardRailContext.SaveChangesAsync();
            return true;
        }
    }
}