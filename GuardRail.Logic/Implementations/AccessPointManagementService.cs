using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;
using GuardRail.Database.Main;
using GuardRail.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Logic.Implementations;

/// <summary>
/// High-level actions for accessPoint management.
/// </summary>
public sealed class AccessPointManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory,
    ILocationCommunicationService locationCommunicationService)
    : IAccessPointManagementService
{
    /// <inheritdoc />
    public async Task CreateNewAccessPoint(
        Guid locationGuid,
        string name,
        AccessPointType accessPointType,
        decimal? latitude,
        decimal? longitude,
        decimal? geoFenceDistance,
        bool requiresAllAccessMethods,
        TimeSpan? accessMethodTimeout,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        await db.AccessPoints.AddAsync(
            new AccessPoint
            {
                LocationGuid = locationGuid,
                Name = name,
                AccessPointType = accessPointType,
                Latitude = latitude,
                Longitude = longitude,
                GeoFenceDistance = geoFenceDistance,
                RequiresAllAccessMethods = requiresAllAccessMethods,
                AccessMethodTimeout = accessMethodTimeout
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AccessPoint>> ListAccessPoints(
        Guid locationId,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        return await db.AccessPoints
            .ToListAsync(
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetAvailableAccessPoints(
        Guid locationId,
        AccessPointType accessPointType,
        TimeSpan timeout,
        CancellationToken cancellationToken) =>
        await locationCommunicationService.GetAvailableAccessPoints(
            locationId,
            accessPointType,
            timeout,
            cancellationToken);
}