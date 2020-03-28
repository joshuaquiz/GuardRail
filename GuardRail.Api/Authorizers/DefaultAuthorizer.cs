using System;
using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.Api.Authorizers
{
    /// <summary>
    /// Everyone gets in all the time!
    /// </summary>
    public sealed class DefaultAuthorizer : IAuthorizer
    {
        private readonly GuardRailLogger _guardRailLogger;

        public DefaultAuthorizer(
            GuardRailLogger guardRailLogger)
        {
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

            await _guardRailLogger.LogAsync($"User {user.FirstName} {user.LastName} is granted access using {accessControlDevice.GetDeviceId()}");
            return true;
        }
    }
}