using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<List<Command>> ListCommands(
        Guid locationId,
        CommandStatus? status,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        return await db.Commands
            .Where(
                x =>
                    x.LocationGuid == locationId
                    && (status == null || x.Status == status))
            .ToListAsync(
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Command> AddNewCommand(
        Guid locationId,
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
                LocationGuid = locationId,
                Type = type,
                ExpiryDate = expiryDate,
                MaxRetries = maxRetries ?? 0,
                Body = body,
                Status = CommandStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                Retries = 0
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        return newCommand.Entity;
    }

    /// <inheritdoc />
    public async Task UpdateCommand(
        Guid commandId,
        CommandStatus status,
        string? response,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var command = await db.Commands
            .FirstOrDefaultAsync(
                x =>
                    x.Guid == commandId,
                cancellationToken);
        if (command == null)
        {
            throw new ArgumentOutOfRangeException(
                nameof(commandId),
                commandId,
                "Command not found.");
        }

        switch (status)
        {
            case CommandStatus.Pending:
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    status,
                    "Command cannot be set to pending.");
            case CommandStatus.InProgress:
                command.StartDate = DateTimeOffset.UtcNow;
                command.Status = CommandStatus.InProgress;
                break;
            case CommandStatus.CompletedSuccessfully:
                command.EndDate = DateTimeOffset.UtcNow;
                command.Status = CommandStatus.CompletedSuccessfully;
                break;
            case CommandStatus.CompletedWithErrors:
                command.EndDate = DateTimeOffset.UtcNow;
                command.Status = CommandStatus.CompletedWithErrors;
                break;
            case CommandStatus.Failed:
                command.EndDate = DateTimeOffset.UtcNow;
                command.Status = CommandStatus.Failed;
                command.Retries += 1;
                break;
            case CommandStatus.Expired:
                command.EndDate = DateTimeOffset.UtcNow;
                command.Status = CommandStatus.Expired;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        if (!string.IsNullOrWhiteSpace(response))
        {
            command.Response = response;
        }

        await db.SaveChangesAsync(
            cancellationToken);
    }
}