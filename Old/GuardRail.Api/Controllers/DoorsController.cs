using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models;
using GuardRail.ApiModels.Requests;
using GuardRail.Core.Data;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers;

[Authorize]
[Route("api/doors")]
[ApiController]
public sealed class DoorsController : ControllerBase
{
    private readonly GuardRailContext _guardRailContext;

    public DoorsController(
        GuardRailContext guardRailContext)
    {
        _guardRailContext = guardRailContext;
    }

    [Route("")]
    [HttpGet]
    public async Task<List<DoorModel>> GetAllDoorsAsync() =>
        await _guardRailContext
            .Doors
            .Select(x =>
                new DoorModel
                {
                    Id = x.Guid,
                    FriendlyName = x.FriendlyName,
                    DeviceId = x.DeviceId,
                    DoorStateRequestType = x.DoorStateRequestType
                })
            .ToListAsync();

    [Route("{doorId:guid}/lock")]
    [HttpPost]
    public async Task<OkResult> LockDoorAsync(Guid doorId)
    {
        var doorFromDatabase = await _guardRailContext
            .Doors
            .SingleAsync(
                x => x.Guid == doorId,
                HttpContext.RequestAborted);
        /*var door = await _doorResolver.GetDoorByDeviceId(
            doorFromDatabase.DeviceId,
            HttpContext.RequestAborted);
        await door.LockAsync(
            HttpContext.RequestAborted);*/
        doorFromDatabase.DoorStateRequestType = DoorStateRequestType.Locked;
        await _guardRailContext.SaveChangesAsync();
        return Ok();
    }

    [Route("{doorId:guid}/unlock")]
    [HttpPost]
    public async Task<OkResult> UnlockDoorAsync(Guid doorId)
    {
        var doorFromDatabase = await _guardRailContext
            .Doors
            .SingleAsync(
                x => x.Guid == doorId,
                HttpContext.RequestAborted);
        /*var door = await _doorResolver.GetDoorByDeviceId(
            doorFromDatabase.DeviceId,
            HttpContext.RequestAborted);
        await door.UnLockAsync(
            HttpContext.RequestAborted);*/
        doorFromDatabase.DoorStateRequestType = DoorStateRequestType.UnLocked;
        await _guardRailContext.SaveChangesAsync();
        return Ok();
    }

    [Route(nameof(IsInGeoFence))]
    [HttpPost]
    public async Task<bool> IsInGeoFence(
        [FromBody] GeoLocationRequest geoLocationRequest,
        CancellationToken cancellationToken)
    {
        var doorFromDatabase = await _guardRailContext
            .Doors
            .Include(x => x.AccessPoint)
            .SingleAsync(
                x => x.Guid == geoLocationRequest.DoorId,
                cancellationToken);
        var distance = DistanceBetweenPoints(
            geoLocationRequest.Latitude,
            geoLocationRequest.Longitude,
            doorFromDatabase.Latitude,
            doorFromDatabase.Longitude);
        return distance <= doorFromDatabase.AccessPoint.DistanceAllowance;
    }

    private const double EarthsRadiusInKilometers = 6371;

    private static double DistanceBetweenPoints(
        double fromLat,
        double fromLong,
        double toLat,
        double toLong)
    {
        return (Math.PI / 180)
               * (Math.Acos(
                      Math.Sin(fromLat)
                      * Math.Sin(toLat)
                      + Math.Cos(fromLat)
                      * Math.Cos(toLat)
                      * Math.Cos(toLong - fromLong)
                  )
                  * EarthsRadiusInKilometers);
    }
}