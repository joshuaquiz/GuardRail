using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.Authorizers.AlwaysAllowAuthorizer
{
    public sealed class AlwaysAllowAuthorizer : IAuthorizer
    {
        public Task<bool> IsDeviceAuthorizedAtLocation(IUser user, IAccessControlDevice accessControlDevice)
        {
            return Task.FromResult(true);
        }
    }
}