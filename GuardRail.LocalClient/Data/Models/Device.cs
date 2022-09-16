using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models;

/// <inheritdoc />
public sealed class Device : Core.DataModels.Device
{
    /// <summary>
    /// The ID used for EF.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The user that owns the device.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// EF creation helper.
    /// </summary>
    /// <param name="builder">EF model builder.</param>
    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<Device>()
            ?.HasKey(x => x.Id);
        builder
            ?.Entity<Device>()
            ?.HasIndex(x => x.Guid);
        builder
            ?.Entity<Device>()
            ?.HasIndex(x => x.DeviceId);
        builder
            ?.Entity<Device>()
            ?.HasIndex(x => x.ByteId);
        builder
            ?.Entity<Device>()
            ?.HasOne<User>();
    }
}