using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models;

/// <inheritdoc />
public sealed class UserGroup : Core.DataModels.UserGroup
{
    /// <summary>
    /// The ID used for EF.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The users in the group.
    /// </summary>
    public List<User> Users { get; } = new();

    /// <summary>
    /// The AccessPoints the group has direct access to.
    /// </summary>
    public List<AccessPoint> AccessPoints { get; } = new();

    /// <summary>
    /// The AccessPointGroups the user group has access to.
    /// </summary>
    public List<AccessPointGroup> AccessPointGroups { get; } = new();

    /// <summary>
    /// EF creation helper.
    /// </summary>
    /// <param name="builder">EF model builder.</param>
    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<UserGroup>()
            ?.HasKey(x => x.Id);
        builder
            ?.Entity<UserGroup>()
            ?.HasIndex(x => x.Guid);
        builder
            ?.Entity<UserGroup>()
            ?.HasMany<User>();
        builder
            ?.Entity<UserGroup>()
            ?.HasMany<AccessPoint>();
        builder
            ?.Entity<UserGroup>()
            ?.HasMany<AccessPointGroup>();
    }
}