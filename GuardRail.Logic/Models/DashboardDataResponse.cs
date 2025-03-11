using System.Collections.Generic;
using GuardRail.Core.Models.Models;

namespace GuardRail.Logic.Models;

/// <summary>
/// The data needed by a UI to render the dashboard.
/// </summary>
/// <param name="Accounts">The accounts the user has access to.</param>
/// <param name="SupportedDevices">The list of devices that are supported by GuardRail.</param>
public sealed record DashboardDataResponse(
    IReadOnlyCollection<Account> Accounts,
    IReadOnlyDictionary<string, string> SupportedDevices);