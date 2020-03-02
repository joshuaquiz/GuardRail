using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Determines if the device is valid at the location.
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Determines if the device is valid at the location.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="accessControlDevice"></param>
        /// <returns></returns>
        Task<bool> IsDeviceAuthorizedAtLocation(
            IDevice device,
            IAccessControlDevice accessControlDevice);
    }
}