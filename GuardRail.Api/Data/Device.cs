using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class Device : Core.DataModels.Device
    {
        public int Id { get; set; }

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
            builder
                ?.Entity<Device>()
                ?.HasOne<User>();
        }
    }
}