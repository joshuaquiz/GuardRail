using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;
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
        private readonly GuardRailContext _guardRailContext;
        private readonly IGuardRailLogger _guardRailLogger;

        /// <summary>
        /// Builds a Acr1252UFactory.
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="sCardContext"></param>
        /// <param name="logger"></param>
        /// <param name="guardRailContext"></param>
        /// <param name="guardRailLogger"></param>
        public Acr1252UFactory(
            IEventBus eventBus,
            ISCardContext sCardContext,
            ILogger logger,
            GuardRailContext guardRailContext,
            IGuardRailLogger guardRailLogger)
        {
            _eventBus = eventBus;
            _sCardContext = sCardContext;
            _logger = logger;
            _guardRailContext = guardRailContext;
            _guardRailLogger = guardRailLogger;
        }

        /// <summary>
        /// Get ACR1252U access control devices.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<IAccessControlDevice>> GetAccessControlDevices()
        {
            _sCardContext.Establish(SCardScope.System);
            var readers = _sCardContext.GetReaders();
            var devices = new List<IAccessControlDevice>();
            devices.AddRange(
                readers.Where(x =>
                        x.Contains("PICC", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains("ACR", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains("1252", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x =>
                        Acr1252PiccDevice.Create(
                            x,
                            _eventBus,
                            _sCardContext,
                            _logger,
                            _guardRailContext)));
            devices.AddRange(
                readers.Where(x =>
                        x.Contains("SAM", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains("ACR", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains("1252", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x =>
                        Acr1252SamDevice.Create(
                            x,
                            _eventBus,
                            _sCardContext,
                            _logger,
                            _guardRailContext)));
            _sCardContext.Release();
            await _guardRailLogger.LogAsync($"{devices.Count} access control devices were found");
            foreach (var accessControlDevice in devices)
            {
                var acdId = await accessControlDevice.GetDeviceId();
                var acd = await _guardRailContext.AccessControlDevices.SingleOrDefaultAsync(x => x.DeviceId == acdId);
                if (acd == null)
                {
                    await _guardRailContext.AccessControlDevices.AddAsync(
                        new AccessControlDevice
                        {
                            DeviceId = acdId,
                            IsConfigured = false
                        });
                    await _guardRailContext.SaveChangesAsync();

                }
            }

            return new ReadOnlyCollection<IAccessControlDevice>(devices);
        }
    }
}