using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Api.Models;
using GuardRail.Core.Enums;
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
                    Id = x.Id,
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
                x => x.Id == doorId,
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
                x => x.Id == doorId,
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
}