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
    [Microsoft.AspNetCore.Mvc.Route("api/devices")]
    [ApiController]
    public sealed class DevicesController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public DevicesController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<List<DeviceModel>> GetAllDevicesAsync() =>
            await _guardRailContext
                .Devices
                .Select(x =>
                    new DeviceModel
                    {
                        Id = x.Id,
                        FriendlyName = x.FriendlyName,
                        IsConfigured = x.IsConfigured
                    })
                .ToListAsync();

        [Microsoft.AspNetCore.Mvc.Route("{id}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<DeviceModel> GetDeviceAsync(
            Guid id)
        {
            var device = await _guardRailContext
                .Devices
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    HttpContext.RequestAborted);
            return new DeviceModel
            {
                Id = device.Id,
                FriendlyName = device.FriendlyName,
                IsConfigured = device.IsConfigured,
                User = device.User == null
                    ? null
                    : new UserModel
                    {
                        Id = device.User.Id,
                        FirstName = device.User.FirstName,
                        LastName = device.User.LastName
                    }
            };
        }

        [Microsoft.AspNetCore.Mvc.Route("{id}")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task UpdateDeviceAsync(
            DeviceModel deviceModel)
        {
            if (deviceModel == null)
            {
                throw new ArgumentNullException(nameof(deviceModel));
            }

            var device = await _guardRailContext
                .Devices
                .SingleAsync(
                    x => x.Id == deviceModel.Id,
                    HttpContext.RequestAborted);
            device.FriendlyName = deviceModel.FriendlyName;
            device.IsConfigured = deviceModel.IsConfigured;
            device.User = await _guardRailContext
                .Users
                .SingleAsync(x => x.Id == deviceModel.User.Id);
            await _guardRailContext.SaveChangesAsync(HttpContext.RequestAborted);
        }
    }
}