using System;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class UserAccess
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public AccessControlDevice AccessControlDevice { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<UserAccess>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<UserAccess>()
                ?.HasOne<User>();
            builder
                ?.Entity<UserAccess>()
                ?.HasOne<AccessControlDevice>();
        }
    }
}