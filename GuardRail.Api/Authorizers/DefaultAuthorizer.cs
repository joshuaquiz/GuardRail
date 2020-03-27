using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Authorizers
{
    /// <summary>
    /// Everyone gets in all the time!
    /// </summary>
    public sealed class DefaultAuthorizer : IAuthorizer
    {
        private readonly IEventBus _eventBus;
        private readonly GuardRailContext _guardRailContext;
        private readonly GuardRailHub _guardRailHub;
        private readonly GuardRailLogger _guardRailLogger;

        public DefaultAuthorizer(
            IEventBus eventBus,
            GuardRailContext guardRailContext,
            GuardRailHub guardRailHub,
            GuardRailLogger guardRailLogger)
        {
            _eventBus = eventBus;
            _guardRailContext = guardRailContext;
            _guardRailHub = guardRailHub;
            _guardRailLogger = guardRailLogger;
            Setup();
        }

        private void Setup()
        {
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
                            x => x.DeviceId == acdId);
                    if (!accessControlDevice.IsConfigured)
                    {
                        var log = $"Access control device {(string.IsNullOrWhiteSpace(accessControlDevice.FriendlyName) ? accessControlDevice.DeviceId : accessControlDevice.FriendlyName)} is not configured";
                        await _guardRailLogger.LogAsync(log);
                        await _guardRailHub.SendLogAsync(log);
                        await newItem.AccessControlDevice.PresentNoAccessGranted(log);
                    }

                    var device =
                        await _guardRailContext.Devices.SingleOrDefaultAsync(
                            x => x.DeviceId == newItem.Device.DeviceId);
                    if (!device.IsConfigured)
                    {
                        var log = $"Device {(string.IsNullOrWhiteSpace(device.FriendlyName) ? device.DeviceId : device.FriendlyName)} is not configured";
                        await _guardRailLogger.LogAsync(log);
                        await _guardRailHub.SendLogAsync(log);
                        await newItem.AccessControlDevice.PresentNoAccessGranted(log);
                    }

                    if (await IsDeviceAuthorizedAtLocation(
                        device.User,
                        newItem.AccessControlDevice))
                    {
                        foreach (var door in accessControlDevice.Doors)
                        {
                            await door.UnLockAsync(CancellationToken.None);
                        }
                    }
                    else
                    {
                        var log = $"{device.User.FirstName} {device.User.LastName} is not allowed entry at {accessControlDevice.FriendlyName}";
                        await _guardRailLogger.LogAsync(log);
                        await _guardRailHub.SendLogAsync(log);
                        await newItem
                            .AccessControlDevice
                            .PresentNoAccessGranted(
                                log);
                    }
                    _eventBus.AccessAuthorizationEvents.Remove(newItem);
                }
            };
        }

        /// <summary>
        /// Is device allowed? Yes!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessControlDevice"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceAuthorizedAtLocation(
            IUser user,
            IAccessControlDevice accessControlDevice)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    nameof(user));
            }

            if (accessControlDevice == null)
            {
                throw new ArgumentNullException(
                    nameof(accessControlDevice));
            }

            await _guardRailLogger.LogAsync($"User {user.FirstName} {user.LastName} is granted access using {accessControlDevice.GetDeviceId()}");
            return true;
        }
    }
}