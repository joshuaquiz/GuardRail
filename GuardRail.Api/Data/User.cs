using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public sealed class User : Core.DataModels.User
{
    public int Id { get; set; }

    public List<UserAccessMethods> UserAccessMethods { get; set; }

    public List<DoorUserAccess> DoorUserAccesses { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<User>()
            ?.HasKey(b => b.Guid);
        builder
            ?.Entity<User>()
            ?.HasMany<Device>();
        builder
            ?.Entity<User>()
            ?.HasMany<UserAccessMethods>();
        builder
            ?.Entity<User>()
            ?.HasMany<DoorUserAccess>();
    }
}