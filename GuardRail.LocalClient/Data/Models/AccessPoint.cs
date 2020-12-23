using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models
{
    /// <inheritdoc />
    public sealed class AccessPoint : Core.DataModels.AccessPoint
    {
        /// <summary>
        /// The ID used for EF.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The door attached to this access point.
        /// </summary>
        public Door Door { get; set; }

        /// <summary>
        /// The users with direct access.
        /// </summary>
        public List<User> Users { get; } = new List<User>();

        /// <summary>
        /// The UserGroups this access point is linked to.
        /// </summary>
        public List<UserGroup> UserGroups { get; } = new List<UserGroup>();

        /// <summary>
        /// The AccessPointGroups this access point is linked to.
        /// </summary>
        public List<AccessPointGroup> AccessPointGroups { get; } = new List<AccessPointGroup>();

        /// <summary>
        /// EF creation helper.
        /// </summary>
        /// <param name="builder">EF model builder.</param>
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<AccessPoint>()
                .HasKey(x => x.Id);
            builder
                .Entity<AccessPoint>()
                .HasIndex(x => x.Guid);
            builder
                .Entity<AccessPoint>()
                .HasIndex(x => x.DeviceId);
            builder
                .Entity<AccessPoint>()
                .HasOne<Door>();
            builder
                .Entity<AccessPoint>()
                .HasMany<User>();
            builder
                .Entity<AccessPoint>()
                .HasMany<UserGroup>();
            builder
                .Entity<AccessPoint>()
                .HasMany<AccessPointGroup>();
        }
    }
}