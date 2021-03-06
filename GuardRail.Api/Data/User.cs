﻿using System;
using System.Collections.Generic;
using GuardRail.Core;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data
{
    public sealed class User : IUser
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Device> Devices { get; set; }

        public List<DoorUserAccess> DoorUserAccesses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder
                ?.Entity<User>()
                ?.HasKey(b => b.Id);
            builder
                ?.Entity<User>()
                ?.HasMany<Device>();
            builder
                ?.Entity<User>()
                ?.HasMany<DoorUserAccess>();
        }
    }
}