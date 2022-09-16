using System;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public sealed class DoorUserAccess
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public int DoorId { get; set; }

    public User User { get; set; }

    public Door Door { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<DoorUserAccess>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<DoorUserAccess>()
            ?.HasKey(x => new { x.UserId, x.DoorId });
        builder
            ?.Entity<DoorUserAccess>()
            ?.HasOne(x => x.User)
            ?.WithMany(x => x.DoorUserAccesses)
            ?.HasForeignKey(x => x.UserId);
        builder
            ?.Entity<DoorUserAccess>()
            ?.HasOne(x => x.Door)
            ?.WithMany(x => x.DoorUserAccesses)
            ?.HasForeignKey(x => x.DoorId);
    }
}