using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public class Door : Core.DataModels.Door
{
    public Guid Id { get; set; }

    public string FriendlyName { get; set; }

    /*public AccessControlDevice AccessControlDevice { get; set; }*/

    public List<DoorUserAccess> DoorUserAccesses { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<Door>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<Door>()
            ?.HasIndex(b => b.DeviceId);
        /*builder
            ?.Entity<Door>()
            ?.HasOne<AccessControlDevice>();*/
        builder
            ?.Entity<Door>()
            ?.HasMany<DoorUserAccess>();
    }

    public Task<string> GetDeviceId() =>
        Task.FromResult(DeviceId);

    public Task<DoorStateRequestType> GetLockedStatus(CancellationToken cancellationToken) =>
        Task.FromResult(DoorStateRequestType);

    public Task LockAsync(CancellationToken cancellationToken)
    {
        DoorStateRequestType = DoorStateRequestType.Locked;
        return Task.CompletedTask;
    }

    public Task UnLockAsync(CancellationToken cancellationToken)
    {
        DoorStateRequestType = DoorStateRequestType.UnLocked;
        return Task.CompletedTask;
    }
}