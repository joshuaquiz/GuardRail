using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Local
{
    public class GuardRailContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=LocalDataStore.db");
    }
}