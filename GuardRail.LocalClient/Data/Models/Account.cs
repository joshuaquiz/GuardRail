using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.LocalClient.Data.Models;

/// <inheritdoc />
public class Account : Core.DataModels.Account
{
    /// <summary>
    /// The ID used for EF.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Users on the account.
    /// </summary>
    public List<User> Users { get; } = new();

    /// <summary>
    /// EF creation helper.
    /// </summary>
    /// <param name="builder">EF model builder.</param>
    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<Account>()
            ?.HasKey(x => x.Id);
        builder
            ?.Entity<Account>()
            ?.HasIndex(x => x.Guid);
        builder
            ?.Entity<Account>()
            ?.HasMany<User>();
    }
}