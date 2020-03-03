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
        private readonly ISCardContext _sCardContext;

        /// <summary>
        /// Builds a Acr1252UFactory.
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="sCardContext"></param>
        public Acr1252UFactory(
            IEventBus eventBus,
            ISCardContext sCardContext)
        {
            _eventBus = eventBus;
            _sCardContext = sCardContext;
        }

        /// <summary>
        /// Get ACR1252U access control devices.
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyCollection<IAccessControlDevice>> GetAccessControlDevices() =>
            Task.Run(() =>
            {
                _sCardContext.Establish(SCardScope.System);
                return (IReadOnlyCollection<IAccessControlDevice>) new ReadOnlyCollection<IAccessControlDevice>(
                    _sCardContext
                        .GetReaders()
                        .Select(x => Acr1252UAccessControlDevice.Create(x, _eventBus, _sCardContext))
                        .ToList());
            });
    }
}