using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    /// <summary>
    /// Database context for the project.
    /// </summary>
    public class GuardRailContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public GuardRailContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Device> Devices { get; set; }
    }
}