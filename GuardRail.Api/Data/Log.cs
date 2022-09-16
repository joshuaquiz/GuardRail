using System;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Data;

public sealed class Log
{
    public Guid Id { get; set; }

    public DateTimeOffset DateTime { get; set; }

    public string LogMessage { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder
            ?.Entity<Log>()
            ?.HasKey(b => b.Id);
        builder
            ?.Entity<Log>()
            ?.HasIndex(b => b.DateTime);
    }
}