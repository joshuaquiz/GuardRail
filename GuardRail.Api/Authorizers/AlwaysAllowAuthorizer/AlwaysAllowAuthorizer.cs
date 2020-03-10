using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.Api.Authorizers.AlwaysAllowAuthorizer
{
    /// <summary>
    /// Everyone gets in all the time!
    /// </summary>
    public sealed class AlwaysAllowAuthorizer : IAuthorizer
    {
        /// <summary>
        /// Is device allowed? Yes!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessControlDevice"></param>
        /// <returns></returns>
        public Task<bool> IsDeviceAuthorizedAtLocation(
            IUser user,
            IAccessControlDevice accessControlDevice)
        {
            return Task.FromResult(true);
        }
    }
}