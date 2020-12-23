using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models
{
    /// <inheritdoc />
    public sealed class Door : Core.DataModels.Door
    {
        /// <summary>
        /// The ID used for EF.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID for the related access point.
        /// </summary>
        public int AccessPointId { get; set; }

        /// <summary>
        /// The AccessPoint linked to the door.
        /// </summary>
        public AccessPoint AccessPoint { get; set; }

        /// <summary>
        /// EF creation helper.
        /// </summary>
        /// <param name="builder">EF model builder.</param>
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Door>()
                .HasKey(x => x.Id);
            builder
                .Entity<Door>()
                .HasIndex(x => x.Guid);
            builder
                .Entity<Door>()
                .HasIndex(x => x.DeviceId);
            builder
                .Entity<Door>()
                .HasOne<AccessPoint>();
        }
    }
}