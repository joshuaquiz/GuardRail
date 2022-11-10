using System;
using GuardRail.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public sealed class UserAccessMethods
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public UnlockRequestType UnlockRequestType { get; set; }

    public byte[] Data { get; set; }

    public User User { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<UserAccessMethods>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<UserAccessMethods>()
            ?.HasKey(x => new { x.UnlockRequestType, x.Data });
        builder
            ?.Entity<UserAccessMethods>()
            ?.HasOne(x => x.User)
            ?.WithMany(x => x.UserAccessMethods)
            ?.HasForeignKey(x => x.UserId);
    }
}