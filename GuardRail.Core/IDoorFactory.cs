using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuardRail.Core
{
    /// <summary>
    /// Used to generate a list of <see cref="IDoor"/>.
    /// </summary>
    public interface IDoorFactory
    {
        /// <summary>
        /// Gets a list of <see cref="IDoor"/>.
        /// </summary>
        /// <returns><see cref="IDoor"/></returns>
        Task<IReadOnlyCollection<IDoor>> GetDoors();
    }
}