using System.Collections.Generic;
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
        private readonly IEnumerable<IDoorFactory> _doorFactories;
        private readonly IEnumerable<IAccessControlFactory> _accessControlFactories;
        private readonly IDoorResolver _doorResolver;
        private readonly GuardRailContext _guardRailContext;

        public CoordinatorService(
            ILogger logger,
            IEnumerable<IDoorFactory> doorFactories,
            IEnumerable<IAccessControlFactory> accessControlFactories,
            IDoorResolver doorResolver,
            GuardRailContext guardRailContext)
        {
            _logger = logger;
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
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "asdf",
                Password = "asdf"
            };
            var role = new Role
            {
                Name = "Admin",
                Users = new List<User>
                {
                    user
                }
            };
            role.AccessControlDeviceRoles =
                (await _guardRailContext.AccessControlDevices.ToListAsync(cancellationToken))
                .Select(x =>
                    new AccessControlDeviceRole
                    {
                        AccessControlDevice = x,
                        Role = role
                    })
                .ToList();
            await _guardRailContext.Users.AddAsync(user, cancellationToken);
            await _guardRailContext.Roles.AddAsync(role, cancellationToken);
            await _guardRailContext.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}