using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using GuardRail.Api.Controllers.Models;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Mvc.Route("api/acd")]
    [ApiController]
    public sealed class AccessControlDevicesController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public AccessControlDevicesController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<List<AccessControlDeviceModel>> GetAllAcdsAsync() =>
            await _guardRailContext
                .AccessControlDevices
                .Select(x =>
                    new AccessControlDeviceModel
                    {
                        Id = x.Id,
                        FriendlyName = x.FriendlyName,
                        DeviceId = x.DeviceId,
                        IsConfigured = x.IsConfigured
                    })
                .ToListAsync();

        [Microsoft.AspNetCore.Mvc.Route("{id}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<AccessControlDeviceModel> GetAcdsAsync(
            Guid id)
        {
            var acd = await _guardRailContext
                .AccessControlDevices
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    HttpContext.RequestAborted);
            return new AccessControlDeviceModel
            {
                Id = acd.Id,
                FriendlyName = acd.FriendlyName,
                DeviceId = acd.DeviceId,
                IsConfigured = acd.IsConfigured,
                Doors = (acd
                        .Doors
                        ?.Select(x =>
                            new DoorModel
                            {
                                Id = x.Id,
                                DeviceId = x.DeviceId,
                                FriendlyName = x.FriendlyName,
                                LockedStatus = x.LockedStatus
                            }) ?? Enumerable.Empty<DoorModel>())
                    .ToList()
            };
        }

        [Microsoft.AspNetCore.Mvc.Route("{id}")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task UpdateAcdAsync(
            AccessControlDeviceModel accessControlDeviceModel)
        {
            if (accessControlDeviceModel == null)
            {
                throw new ArgumentNullException(nameof(accessControlDeviceModel));
            }

            var accessControlDevice = await _guardRailContext
                .AccessControlDevices
                .SingleAsync(
                    x => x.Id == accessControlDeviceModel.Id,
                    HttpContext.RequestAborted);
            accessControlDevice.FriendlyName = accessControlDeviceModel.FriendlyName;
            accessControlDevice.IsConfigured = accessControlDeviceModel.IsConfigured;
            accessControlDevice.Doors = await _guardRailContext
                .Doors
                .Where(x => accessControlDeviceModel.Doors.Any(d => d.Id == x.Id))
                .ToListAsync(HttpContext.RequestAborted);
            await _guardRailContext.SaveChangesAsync(HttpContext.RequestAborted);
        }
    }
}