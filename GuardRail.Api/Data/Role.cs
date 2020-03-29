using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }

        public List<AccessControlDeviceRole> AccessControlDeviceRoles { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<Role>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<Role>()
                ?.HasMany<User>();
            builder
                ?.Entity<Role>()
                ?.HasMany<AccessControlDeviceRole>();
        }
    }
}