using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class Door : IDoor
    {
        public Guid Id { get; set; }

        public string DeviceId { get; set; }

        public string FriendlyName { get; set; }

        public LockedStatus LockedStatus { get; set; }

        public bool IsConfigured { get; set; }

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
}