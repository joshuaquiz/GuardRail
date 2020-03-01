using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GuardRail.Definitions;
using PCSC;

namespace GuardRail.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// Factory for ACR1252U access control devices.
    /// </summary>
    public sealed class Acr1252UFactory : IAccessControlFactory
    {
        /// <summary>
        /// Get ACR1252U access control devices.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IAccessControlDevice> GetAccessControlDevices()
        {
            using var sCardContext = new SCardContext();
            sCardContext.Establish(SCardScope.System);
            return new ReadOnlyCollection<IAccessControlDevice>(
                sCardContext
                    .GetReaders()
                    .Select(Acr1252U.Create)
                    .ToList());
        }
    }
}