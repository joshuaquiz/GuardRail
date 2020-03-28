using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    /// <summary>
    /// Database context for the project.
    /// </summary>
    public class GuardRailContext : DbContext
    {
        public GuardRailContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<DoorUserAccess> DoorUserAccesses { get; set; }

        public DbSet<AccessControlDevice> AccessControlDevices { get; set; }

        public DbSet<Door> Doors { get; set; }

        public DbSet<Log> Logs { get; set; }
    }
}