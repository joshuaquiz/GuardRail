using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using GuardRail.Api.Controllers.Models;
using GuardRail.Api.Data;
using GuardRail.Core;
using GuardRail.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Mvc.Route("api/doors")]
    [ApiController]
    public sealed class DoorsController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;
        private readonly IDoorResolver _doorResolver;

        public DoorsController(
            GuardRailContext guardRailContext,
            IDoorResolver doorResolver)
        {
            _guardRailContext = guardRailContext;
            _doorResolver = doorResolver;
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<List<DoorModel>> GetAllDoorsAsync() =>
            await _guardRailContext
                .Doors
                .Select(x =>
                    new DoorModel
                    {
                        Id = x.Id,
                        FriendlyName = x.FriendlyName,
                        DeviceId = x.DeviceId,
                        LockedStatus = x.LockedStatus
                    })
                .ToListAsync();

        [Microsoft.AspNetCore.Mvc.Route("{doorId}/lock")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<OkResult> LockDoorAsync(Guid doorId)
        {
            var doorFromDatabase = await _guardRailContext
                .Doors
                .SingleAsync(
                    x => x.Id == doorId,
                    HttpContext.RequestAborted);
            var door = await _doorResolver.GetDoorByDeviceId(
                doorFromDatabase.DeviceId,
                HttpContext.RequestAborted);
            /*await door.LockAsync(
                HttpContext.RequestAborted);*/
            doorFromDatabase.LockedStatus = LockedStatus.Locked;
            await _guardRailContext.SaveChangesAsync();
            return Ok();
        }

        [Microsoft.AspNetCore.Mvc.Route("{doorId}/unlock")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<OkResult> UnlockDoorAsync(Guid doorId)
        {
            var doorFromDatabase = await _guardRailContext
                .Doors
                .SingleAsync(
                    x => x.Id == doorId,
                    HttpContext.RequestAborted);
            var door = await _doorResolver.GetDoorByDeviceId(
                doorFromDatabase.DeviceId,
                HttpContext.RequestAborted);
            /*await door.UnLockAsync(
                HttpContext.RequestAborted);*/
            doorFromDatabase.LockedStatus = LockedStatus.UnLocked;
            await _guardRailContext.SaveChangesAsync();
            return Ok();
        }
    }
}