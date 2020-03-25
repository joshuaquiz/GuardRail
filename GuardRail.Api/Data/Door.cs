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

        public AccessControlDevice AccessControlDevice { get; set; }

        public List<User> Users { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<AccessControlDevice>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<AccessControlDevice>()
                ?.HasIndex(b => b.DeviceId);
        }

        public Task<string> GetDeviceId() =>
            Task.FromResult(DeviceId);

        public Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken) =>
            Task.FromResult(LockedStatus);

        public Task Lock(CancellationToken cancellationToken)
        {
            LockedStatus = LockedStatus.Locked;
            return Task.CompletedTask;
        }

        public Task UnLock(CancellationToken cancellationToken)
        {
            LockedStatus = LockedStatus.UnLocked;
            return Task.CompletedTask;
        }
    }
}