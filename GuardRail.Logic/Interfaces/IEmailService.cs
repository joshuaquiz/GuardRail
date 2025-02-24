using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Interfaces;

/// <summary>
/// An interface defining high-level actions for email management.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a single email to a single recipient.
    /// </summary>
    /// <param name="recipient">The user getting the email.</param>
    /// <param name="emailTemplateType">The type of email to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    public Task SendTransactionalEmail(
        User recipient,
        EmailTemplateType emailTemplateType,
        CancellationToken cancellationToken);
}