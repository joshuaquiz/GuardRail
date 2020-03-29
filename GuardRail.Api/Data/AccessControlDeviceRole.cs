using System;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class AccessControlDeviceRole
    {
        public Guid Id { get; set; }

        public AccessControlDevice AccessControlDevice { get; set; }

        public Role Role { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<AccessControlDeviceRole>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<AccessControlDeviceRole>()
                ?.HasOne<AccessControlDevice>();
            builder
                ?.Entity<AccessControlDeviceRole>()
                ?.HasOne<Role>();
        }
    }
}