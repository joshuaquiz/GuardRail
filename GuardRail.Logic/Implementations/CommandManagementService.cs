using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;
using GuardRail.Database.Main;
using GuardRail.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// High-level actions for command management.
/// </summary>
public sealed class CommandManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory)
    : ICommandManagementService
{

    /// <inheritdoc />
    public async Task<Command> AddNewCommand(
        CommandType type,
        DateTimeOffset expiryDate,
        int? maxRetries,
        string body,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var newCommand = await db.Commands.AddAsync(
            new Command
            {
                Type = type,
                ExpiryDate = expiryDate,
                MaxRetries = maxRetries ?? 0,
                Body = body,
                Status = CommandStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        return newCommand.Entity;
    }
}