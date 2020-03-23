using System;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class Device : IDevice
    {
        public Guid Id { get; set; }

        public string DeviceId { get; set; }

        public string FriendlyName { get; set; }

        public byte[] ByteId { get; set; }

        public bool IsConfigured { get; set; }

        public User User { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<Device>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<Device>()
                ?.HasIndex(b => b.DeviceId);
            builder
                ?.Entity<Device>()
                ?.HasIndex(b => b.ByteId);
        }
    }
}