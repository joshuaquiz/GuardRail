using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Exceptions;
using GuardRail.Database.Main;
using GuardRail.Logic.Interfaces;
using GuardRail.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// High-level actions for version management.
/// </summary>
public sealed class VersionManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory)
    : IVersionManagementService
{
    /// <inheritdoc />
    public async Task<VersionCheckResult> VersionCheck(
        Version version,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var newestVersion = await db.VersionHistories
            .OrderByDescending(
                x =>
                    x.Version)
            .FirstAsync(
                cancellationToken);
        if (newestVersion.Version < version)
        {
            throw new InvalidVersionException(
                $"The provided version '{version}' was greater than the known latest version '{newestVersion.Version}'");
        }

        if (newestVersion.Version == version)
        {
            return new VersionCheckResult(
                true,
                null,
                null,
                null);
        }

        return new VersionCheckResult(
            false,
            newestVersion.IsRequired,
            newestVersion.Version,
            // TODO: Get URL.
            new List<KeyValuePair<string, string>>());

    }
}