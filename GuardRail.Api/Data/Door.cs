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

    public Task<DoorStatus> GetLockedStatus(CancellationToken cancellationToken) =>
        Task.FromResult(DoorStatus);

    public Task LockAsync(CancellationToken cancellationToken)
    {
        DoorStatus = DoorStatus.Locked;
        return Task.CompletedTask;
    }

    public Task UnLockAsync(CancellationToken cancellationToken)
    {
        DoorStatus = DoorStatus.UnLocked;
        return Task.CompletedTask;
    }
}