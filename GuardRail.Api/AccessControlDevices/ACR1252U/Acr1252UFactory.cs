using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Core;
using PCSC;
using Serilog;

namespace GuardRail.Api.AccessControlDevices.ACR1252U
{
    /// <summary>
    /// Factory for ACR1252U access control devices.
    /// </summary>
    public sealed class Acr1252UFactory : IAccessControlFactory
    {
        private readonly IEventBus _eventBus;
        private readonly ISCardContext _sCardContext;
        private readonly ILogger _logger;
        private readonly IDeviceProvider _deviceProvider;

        /// <summary>
        /// Builds a Acr1252UFactory.
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="sCardContext"></param>
        /// <param name="logger"></param>
        /// <param name="deviceProvider"></param>
        public Acr1252UFactory(
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger,
            IDeviceProvider deviceProvider)
        {
            _eventBus = eventBus;
            _sCardContext = sCardContext;
            _logger = logger;
            _deviceProvider = deviceProvider;
        }

        /// <summary>
        /// Get ACR1252U access control devices.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<IAccessControlDevice>> GetAccessControlDevices() =>
            await Task.Run(() =>
            {
                _sCardContext.Establish(SCardScope.System);
                var readers = _sCardContext.GetReaders();
                var devices = new List<IAccessControlDevice>();
                devices.AddRange(
                    readers.Where(x => x.Contains("PICC", StringComparison.InvariantCultureIgnoreCase))
                        .Select(x =>
                            Acr1252PiccDevice.Create(
                                x,
                                _eventBus,
                                _sCardContext,
                                _logger,
                                _deviceProvider)));
                devices.AddRange(
                    readers.Where(x => x.Contains("SAM", StringComparison.InvariantCultureIgnoreCase))
                        .Select(x =>
                            Acr1252SamDevice.Create(
                                x,
                                _eventBus,
                                _sCardContext,
                                _logger,
                                _deviceProvider)));
                _sCardContext.Release();
                return new ReadOnlyCollection<IAccessControlDevice>(devices);
            });
    }
}