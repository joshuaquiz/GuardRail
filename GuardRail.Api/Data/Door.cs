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

    public AccessControlDevice AccessControlDevice { get; set; }

    public List<DoorUserAccess> DoorUserAccesses { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<Door>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<Door>()
            ?.HasIndex(b => b.DeviceId);
        builder
            ?.Entity<Door>()
            ?.HasOne<AccessControlDevice>();
        builder
            ?.Entity<Door>()
            ?.HasMany<DoorUserAccess>();
    }

    public Task<string> GetDeviceId() =>
        Task.FromResult(DeviceId);

    public Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken) =>
        Task.FromResult(LockedStatus);

    public Task LockAsync(CancellationToken cancellationToken)
    {
        LockedStatus = LockedStatus.Locked;
        return Task.CompletedTask;
    }

    public Task UnLockAsync(CancellationToken cancellationToken)
    {
        LockedStatus = LockedStatus.UnLocked;
        return Task.CompletedTask;
    }
}