using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Used to generate a list of <see cref="IAccessControlDevice"/>.
    /// </summary>
    public interface IAccessControlFactory
    {
        /// <summary>
        /// Gets a list of <see cref="IAccessControlDevice"/>.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<IAccessControlDevice>> GetAccessControlDevices();
    }
}