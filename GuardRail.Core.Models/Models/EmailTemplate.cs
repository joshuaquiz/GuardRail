using System;
using GuardRail.Core.Enums;

namespace GuardRail.Core.Models.Models;

/// <summary>
/// An email template.
/// </summary>
public class EmailTemplate
{
    /// <summary>
    /// The global ID for the item.
    /// Guid is to be used in all systems for the global ID.
    /// This value is set automatically and should not be passed in for adds.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The ID of the account.
    /// </summary>
    public required Guid AccountGuid { get; set; }

    /// <summary>
    /// The email template type.
    /// </summary>
    public required EmailTemplateType EmailTemplateType { get; set; }

    /// <summary>
    /// The email's subject.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// The email's body.
    /// </summary>
    public required string Body { get; set; }
}