using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GuardRail.Api
{
    public sealed class CoordinatorService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IEventBus _eventBus;
        private readonly IAuthorizer _authorizer;
        private readonly IEnumerable<IDoorFactory> _doorFactories;
        private readonly IEnumerable<IAccessControlFactory> _accessControlFactories;
        private readonly GuardRailContext _guardRailContext;

        public CoordinatorService(
            ILogger logger,
            IEventBus eventBus,
            IAuthorizer authorizer,
            IEnumerable<IDoorFactory> doorFactories,
            IEnumerable<IAccessControlFactory> accessControlFactories,
            GuardRailContext guardRailContext)
        {
            _logger = logger;
            _eventBus = eventBus;
            _authorizer = authorizer;
            _doorFactories = doorFactories;
            _accessControlFactories = accessControlFactories;
            _guardRailContext = guardRailContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
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
            await _guardRailContext.Users.AddAsync(
                new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Username = "asdf",
                    Password = "asdf"
                },
                cancellationToken);
            await _guardRailContext.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}