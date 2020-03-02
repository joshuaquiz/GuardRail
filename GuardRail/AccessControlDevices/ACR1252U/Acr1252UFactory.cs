using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Core;
using PCSC;

namespace GuardRail.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// Factory for ACR1252U access control devices.
    /// </summary>
    public sealed class Acr1252UFactory : IAccessControlFactory
    {
        private readonly IEventBus _eventBus;

        /// <summary>
        /// Builds a Acr1252UFactory.
        /// </summary>
        /// <param name="eventBus"></param>
        public Acr1252UFactory(
            IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Get ACR1252U access control devices.
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyCollection<IAccessControlDevice>> GetAccessControlDevices() =>
            Task.Run(() =>
            {
                using var sCardContext = new SCardContext();
                sCardContext.Establish(SCardScope.System);
                return (IReadOnlyCollection<IAccessControlDevice>) new ReadOnlyCollection<IAccessControlDevice>(
                    sCardContext
                        .GetReaders()
                        .Select(x => Acr1252U.Create(x, _eventBus))
                        .ToList());
            });
    }
}