﻿using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Determines if the device is valid at the location.
    /// </summary>
    public interface IAuthorizer
    {
        /// <summary>
        /// Determines if the user is able to get in at the location.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessControlDevice"></param>
        /// <returns></returns>
        Task<bool> IsDeviceAuthorizedAtLocation(
            IUser user,
            IAccessControlDevice accessControlDevice);
    }
}