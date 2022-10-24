using System.Collections.Generic;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;

namespace GuardRail.Core;

/// <summary>
/// Used to generate a list of <see cref="Door"/>.
/// </summary>
public interface IDoorFactory
{
    /// <summary>
    /// Gets a list of <see cref="Door"/>.
    /// </summary>
    /// <returns><see cref="Door"/></returns>
    Task<IReadOnlyCollection<Door>> GetDoors();
}