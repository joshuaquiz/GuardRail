using System.Collections.Generic;

namespace GuardRail.Core
{
    /// <summary>
    /// Used to generate a list of <see cref="IDoor"/>.
    /// </summary>
    public interface IDoorFactory
    {
        /// <summary>
        /// Gets a list of <see cref="IAccessControlDevice"/>.
        /// </summary>
        /// <returns><see cref="IAccessControlDevice"/></returns>
        IReadOnlyCollection<IDoor> GetDoors();
    }
}