using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models.Models;
using GuardRail.Database.Main;
using GuardRail.Logic.Interfaces;
using GuardRail.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// High-level actions for account management.
/// </summary>
public sealed class AccountManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory,
    IUserManagementService userManagementService)
    : IAccountManagementService
{
    /// <inheritdoc />
    public async Task CreateNewAccount(
        string accountName,
        string firstName,
        string lastName,
        string phone,
        string email,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var newAccount = await db.Accounts.AddAsync(
            new Account
            {
                Name = accountName,
                IsActive = true
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        await userManagementService.CreateNewUser(
            newAccount.Entity.Guid,
            firstName,
            lastName,
            phone,
            email,
            UserType.AccountAdmin,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DashboardDataResponse> GetDashboardData(
        User user,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        return new DashboardDataResponse(
            await db.Accounts
                .Where(
                    x =>
                        user.UserType == UserType.SuperAdmin
                        || x.Guid == user.AccountGuid)
                .ToListAsync(
                    cancellationToken),
            Extensions.GetEnumDescriptions<AccessPointType>());
    }
}