using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDoorResolver _doorResolver;
        private readonly GuardRailContext _guardRailContext;

        public CoordinatorService(
            ILogger logger,
            IEventBus eventBus,
            IAuthorizer authorizer,
            IEnumerable<IDoorFactory> doorFactories,
            IEnumerable<IAccessControlFactory> accessControlFactories,
            IDoorResolver doorResolver,
            GuardRailContext guardRailContext)
        {
            _logger = logger;
            _eventBus = eventBus;
            _authorizer = authorizer;
            _doorFactories = doorFactories;
            _accessControlFactories = accessControlFactories;
            _doorResolver = doorResolver;
            _guardRailContext = guardRailContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var doorFactory in _doorFactories)
            {
                foreach (var door in await doorFactory.GetDoors())
                {
                    await _doorResolver.RegisterDoor(door, cancellationToken);
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
                    var acdId = await newItem.AccessControlDevice.GetDeviceId();
                    var accessControlDevice =
                        await _guardRailContext.AccessControlDevices.SingleOrDefaultAsync(
                            x => x.DeviceId == acdId,
                            cancellationToken);
                    if (!accessControlDevice.IsConfigured)
                    {
                        await newItem.AccessControlDevice.PresentNoAccessGranted("Access control device not configured");
                    }

                    var device =
                        await _guardRailContext.Devices.SingleOrDefaultAsync(
                            x => x.DeviceId == newItem.Device.DeviceId,
                            cancellationToken);
                    if (!device.IsConfigured)
                    {
                        await newItem.AccessControlDevice.PresentNoAccessGranted("Device not configured");
                    }

                    if (await _authorizer.IsDeviceAuthorizedAtLocation(
                        device.User,
                        newItem.AccessControlDevice))
                    {
                        foreach (var door in accessControlDevice.Doors)
                        {
                            await door.UnLock(cancellationToken);
                        }
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