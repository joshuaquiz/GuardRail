using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Core;
using GuardRail.Data;
using Serilog;

namespace GuardRail
{
    public sealed class Coordinator
    {
        private readonly ILogger _logger;
        private readonly GuardRailContext _guardRailContext;
        private readonly IEventBus _eventBus;
        private readonly IAuthorizer _authorizer;
        private readonly IEnumerable<IDoorFactory> _doorFactories;
        private readonly IEnumerable<IAccessControlFactory> _accessControlFactories;

        public Coordinator(
            ILogger logger,
            GuardRailContext guardRailContext,
            IEventBus eventBus,
            IAuthorizer authorizer,
            IEnumerable<IDoorFactory> doorFactories,
            IEnumerable<IAccessControlFactory> accessControlFactories)
        {
            _logger = logger;
            _guardRailContext = guardRailContext;
            _eventBus = eventBus;
            _authorizer = authorizer;
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
                    .Select(async x => await x.Init()));
            }

            _logger.Debug("Done loading access control devices");
            _eventBus.AccessAuthorizationEvents.CollectionChanged += async (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add)
                {
                    return;
                }

                foreach (AccessAuthorizationEvent newItem in args.NewItems)
                {
                    if (await _authorizer.IsDeviceAuthorizedAtLocation(
                        null, // TODO: Get User by device.
                        newItem.AccessControlDevice))
                    {
                        // TODO: Get door and unlock it.
                    }
                    else
                    {
                        await newItem
                            .AccessControlDevice
                            .PresentNoAccessGranted(
                                "This user is not allowed at this location");
                    }
                }
            };
        }
    }
}