using System.Collections.Generic;

namespace GuardRail.Definitions
{
    /// <summary>
    /// Used to generate a list of <see cref="IAccessControlDevice"/>.
    /// </summary>
    public interface IAccessControlFactory
    {
        /// <summary>
        /// Gets a list of <see cref="IAccessControlDevice"/>.
        /// </summary>
        /// <returns><see cref="IAccessControlDevice"/></returns>
        IReadOnlyCollection<IAccessControlDevice> GetAccessControlDevices();
    }
}