using System;

namespace GuardRail.Core.Data.Interfaces;

/// <summary>
/// An item linked to an account.
/// </summary>
public interface IAccountItem
{
    /// <summary>
    /// The ID of the account.
    /// </summary>
    public Guid AccountGuid { get; set; }
}