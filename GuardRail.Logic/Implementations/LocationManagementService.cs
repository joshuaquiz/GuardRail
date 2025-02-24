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
/// High-level actions for location management.
/// </summary>
public sealed class LocationManagementService(
    IDbContextFactory<GuardRailDbContext> dbContextFactory)
    : ILocationManagementService
{
    /// <inheritdoc />
    public async Task<Location> CreateNewLocation(
        Guid accountId,
        bool isMobile,
        string name,
        string? description,
        decimal? latitude,
        decimal? longitude,
        decimal? geoFenceDistance,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var newLocation = await db.Locations.AddAsync(
            new Location
            {
                AccountGuid = accountId,
                IsMobile = isMobile,
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                GeoFenceDistance = geoFenceDistance
            },
            cancellationToken);
        await db.SaveChangesAsync(
            cancellationToken);
        return newLocation.Entity;
    }

    /// <inheritdoc />
    public async Task SetCommunicationMethod(
        Guid locationId,
        LocationCallbackType locationCallbackType,
        Uri? uri,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
        var location = await db.Locations
            .SingleAsync(
                x =>
                    x.Guid == locationId,
                cancellationToken);
        location.CallbackType = locationCallbackType;
        location.LocalUri = uri;
        await db.SaveChangesAsync(
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Location>> ListLocations(
        Guid accountId,
        CancellationToken cancellationToken)
    {
         await using var db = await dbContextFactory.CreateDbContextAsync(
            cancellationToken);
         return await db.Locations
             .ToListAsync(
                 cancellationToken);
    }
}