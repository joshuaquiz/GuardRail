using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using GuardRail.Api.Data;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api;

public sealed class InMemoryEventBus : IEventBus
{
    public ObservableCollection<AccessAuthorizationEvent> AccessAuthorizationEvents { get; }

    public InMemoryEventBus(
        IAuthorizer authorizer,
        GuardRailContext guardRailContext,
        GuardRailLogger guardRailLogger)
    {
        AccessAuthorizationEvents = new ObservableCollection<AccessAuthorizationEvent>();
        AccessAuthorizationEvents.CollectionChanged += async (sender, args) =>
        {
            if (args.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (AccessAuthorizationEvent newItem in args.NewItems)
            {
                var acdId = await newItem.AccessControlDevice.GetDeviceId();
                var accessControlDevice =
                    await guardRailContext.AccessControlDevices.SingleOrDefaultAsync(
                        x => x.DeviceId == acdId);
                if (!accessControlDevice.IsConfigured)
                {
                    var log = $"Access control device {(string.IsNullOrWhiteSpace(accessControlDevice.FriendlyName) ? accessControlDevice.DeviceId : accessControlDevice.FriendlyName)} is not configured";
                    await guardRailLogger.LogAsync(log);
                    await newItem.AccessControlDevice.PresentNoAccessGranted(log);
                    AccessAuthorizationEvents.Remove(newItem);
                    return;
                }

                var device =
                    await guardRailContext.Devices.SingleOrDefaultAsync(
                        x => x.DeviceId == newItem.Device.DeviceId);
                if (device == null)
                {
                    var log = $"Device {(string.IsNullOrWhiteSpace(newItem.Device.FriendlyName) ? newItem.Device.DeviceId : newItem.Device.FriendlyName)} is not configured";
                    await guardRailLogger.LogAsync(log);
                    await newItem.AccessControlDevice.PresentNoAccessGranted(log);
                    AccessAuthorizationEvents.Remove(newItem);
                    return;
                }

                if (!device.IsConfigured)
                {
                    var log = $"Device {(string.IsNullOrWhiteSpace(device.FriendlyName) ? device.DeviceId : device.FriendlyName)} is not configured";
                    await guardRailLogger.LogAsync(log);
                    await newItem.AccessControlDevice.PresentNoAccessGranted(log);
                    AccessAuthorizationEvents.Remove(newItem);
                    return;
                }

                if (await authorizer.IsDeviceAuthorizedAtLocation(
                        device.User,
                        newItem.AccessControlDevice))
                {
                    await accessControlDevice.Door.UnLockAsync(CancellationToken.None);
                }
                else
                {
                    var log = $"{device.User.FirstName} {device.User.LastName} is not allowed entry at {accessControlDevice.FriendlyName}";
                    await guardRailLogger.LogAsync(log);
                    await newItem
                        .AccessControlDevice
                        .PresentNoAccessGranted(
                            log);
                }
                AccessAuthorizationEvents.Remove(newItem);
            }
        };
    }
}