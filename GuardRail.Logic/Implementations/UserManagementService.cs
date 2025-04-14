using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Exceptions;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models.Models;
using GuardRail.Database.Main;
using GuardRail.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// High-level actions for user management.
/// </summary>
public sealed class UserManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory,
    IEmailService emailService)
    : IUserManagementService
{
    /// <inheritdoc />
    public async Task<Guid> SignIn(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var hashedPassword = PasswordHelpers.Hash(
            password);
        var user = await db.Users.SingleOrDefaultAsync(
            x =>
                x.Email == email
                && x.Password == hashedPassword,
            cancellationToken);
        if (user is null)
        {
            throw new InvalidEmailPasswordException(
                email);
        }

        foreach (var userAccessToken in db.UserAccessTokens
                     .Where(
                         x =>
                             x.UserGuid == user.Guid
                             && x.IsActive))
        {
            userAccessToken.IsActive = false;
        }

        var newToken = await db.UserAccessTokens.AddAsync(
            new UserAccessToken
            {
                UserGuid = user.Guid,
                DateTime = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromHours(1),
                IsActive = true
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        return newToken.Entity.Guid;
    }

    /// <inheritdoc />
    public async Task<User> CreateNewUser(
        Guid accountId,
        string firstName,
        string lastName,
        string phone,
        string email,
        UserType userType,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var newUser = await db.Users.AddAsync(
            new User
            {
                AccountGuid = accountId,
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Email = email,
                Password = Guid.NewGuid().ToString(),
                PasswordResetDate = DateOnly.FromDateTime(
                    DateTime.UtcNow),
                UserType = userType
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        await emailService.SendTransactionalEmail(
            newUser.Entity,
            EmailTemplateType.NewUser,
            cancellationToken);
        return newUser.Entity;
    }

    /// <inheritdoc />
    public async Task ResetPassword(
        Guid userId,
        string newPassword,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var user = await db.Users.FirstOrDefaultAsync(
            x =>
                x.Guid == userId,
            cancellationToken);
        if (user is null)
        {
            throw new UserCouldNotBeFoundException(
                $"The ID '{userId}' did not return a valid user record.");
        }

        user.Password = PasswordHelpers.Hash(
            newPassword);
        user.PasswordResetDate = DateOnly.FromDateTime(
            DateTime.UtcNow.AddDays(
                90));
        await db.SaveChangesAsync(
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<User>> ListUsers(
        Guid accountId,
        IReadOnlyCollection<string>? tags,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var usersForAccountQuery = db.Users
            .Where(
                x =>
                    x.AccountGuid == accountId);
        if (tags is { Count: > 0 })
        {
            usersForAccountQuery = usersForAccountQuery
                .GroupJoin(
                    db.Tags
                        .Where(
                            y =>
                                y.AccountGuid == accountId
                                && tags.Contains(y.TaggedItemName)),
                    u =>
                        u.Guid,
                    t =>
                        t.TaggedItemGuid,
                    (u, t) =>
                        u);
        }

        return await usersForAccountQuery
            .ToListAsync(
                cancellationToken);
    }
}