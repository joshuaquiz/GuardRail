using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Core;
using Serilog;

namespace GuardRail
{
    public sealed class Coordinator
    {
        private readonly ILogger _logger;
        private readonly IEventBus _eventBus;
        private readonly IEnumerable<IDoorFactory> _doorFactories;
        private readonly IEnumerable<IAccessControlFactory> _accessControlFactories;

        public Coordinator(
            ILogger logger,
            IEventBus eventBus,
            IEnumerable<IDoorFactory> doorFactories,
            IEnumerable<IAccessControlFactory> accessControlFactories)
        {
            _logger = logger;
            _eventBus = eventBus;
            _doorFactories = doorFactories;
            _accessControlFactories = accessControlFactories;
        }

        public async Task StartUp()
        {
            foreach (var doorFactory in _doorFactories)
            {
                foreach (var door in doorFactory.GetDoors())
                {
                    // TODO: Register door.
                }
            }

            _logger.Debug("Starting loading access control devices");
            foreach (var accessControlDevices in _accessControlFactories
                .Select(x => x.GetAccessControlDevices()))
            {
                await Task.WhenAll(
                    (await accessControlDevices)
                    .Select(x => x.Init()));
            }

            _logger.Debug("Done loading access control devices");

            // TODO: Attach authorizer to event handler.
        }
    }
}