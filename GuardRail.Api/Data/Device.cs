using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class Device : IDevice
    {
        public string Id { get; }

        public byte[] ByteId { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<Device>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<Device>()
                ?.HasKey(b => b.Id);
        }
    }
}