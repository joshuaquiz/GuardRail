using System;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Data
{
    /// <summary>
    /// A system user
    /// </summary>
    public class User : IUser
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void OnModelCreating(ModelBuilder builder) =>
            builder
                ?.Entity<User>()
                ?.HasKey(b => b.Id);
    }
}