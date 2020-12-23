using GuardRail.LocalClient.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Local
{
    /// <summary>
    /// The EF core context.
    /// </summary>
    public sealed class GuardRailContext : DbContext
    {
        /// <summary>
        /// The accounts.
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// The users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// The access points.
        /// </summary>
        public DbSet<AccessPoint> AccessPoints { get; set; }

        /// <summary>
        /// The access point groups.
        /// </summary>
        public DbSet<AccessPointGroup> AccessPointGroups { get; set; }

        /// <summary>
        /// The devices.
        /// </summary>
        public DbSet<Device> Devices { get; set; }

        /// <summary>
        /// The doors.
        /// </summary>
        public DbSet<Door> Doors { get; set; }

        /// <summary>
        /// The user groups.
        /// </summary>
        public DbSet<UserGroup> UserGroups { get; set; }

        /// <summary>
        /// The EF core context.
        /// </summary>
        public GuardRailContext()
        {

        }

        /// <summary>
        /// The EF core context.
        /// </summary>
        /// <param name="dbContextOptions">The context options.</param>
        public GuardRailContext(DbContextOptions<GuardRailContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        /// <summary>
        /// OnConfiguring event.
        /// </summary>
        /// <param name="options">The context options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=LocalDataStore.db");
    }
}