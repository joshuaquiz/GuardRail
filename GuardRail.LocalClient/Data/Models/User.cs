using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models
{
    /// <inheritdoc />
    public sealed class User : Core.DataModels.User
    {
        /// <summary>
        /// The ID used for EF.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The account the user is in.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// The devices a user has.
        /// </summary>
        public List<Device> Devices { get; } = new List<Device>();

        /// <summary>
        /// The AccessPoints a user has direct access to.
        /// </summary>
        public List<AccessPoint> AccessPoints { get; } = new List<AccessPoint>();

        /// <summary>
        /// The UserGroups a user is in.
        /// </summary>
        public List<UserGroup> UserGroups { get; } = new List<UserGroup>();

        /// <summary>
        /// The AccessPointGroups a user has access to.
        /// </summary>
        public List<AccessPointGroup> AccessPointGroups { get; } = new List<AccessPointGroup>();

        /// <summary>
        /// EF creation helper.
        /// </summary>
        /// <param name="builder">EF model builder.</param>
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<User>()
                ?.HasKey(x => x.Id);
            builder
                ?.Entity<User>()
                ?.HasIndex(x => x.Guid);
            builder
                ?.Entity<User>()
                ?.HasOne<Account>();
            builder
                ?.Entity<User>()
                ?.HasMany<Device>();
            builder
                ?.Entity<User>()
                ?.HasMany<AccessPoint>();
            builder
                ?.Entity<User>()
                ?.HasMany<UserGroup>();
            builder
                ?.Entity<User>()
                ?.HasMany<AccessPointGroup>();
        }

        /// <summary>
        /// Returns a string that contains text that can be searched against.
        /// </summary>
        /// <returns>string</returns>
        public string GetSearchString() =>
            string.Join(
                " ",
                FirstName,
                LastName,
                Email,
                Phone);
    }
}