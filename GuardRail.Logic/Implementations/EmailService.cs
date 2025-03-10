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
/// An interface defining high-level actions for email management.
/// </summary>
public sealed class EmailService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory)
    : IEmailService
{
    /// <summary>
    /// Sends a single email to a single recipient.
    /// </summary>
    /// <param name="recipient">The user getting the email.</param>
    /// <param name="emailTemplateType">The type of email to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public async Task SendTransactionalEmail(
        User recipient,
        EmailTemplateType emailTemplateType,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var template = await db.EmailTemplates
                           .FirstOrDefaultAsync(
                               x =>
                                   x.AccountGuid == recipient.AccountGuid
                                   && x.EmailTemplateType == emailTemplateType,
                               cancellationToken)
                       ?? await db.EmailTemplates
                           .FirstAsync(
                               x =>
                                   x.AccountGuid == Guid.Empty
                                   && x.EmailTemplateType == emailTemplateType,
                               cancellationToken);
        // TODO: Replace tokens.
        // TODO: Send email.
    }
}