﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public sealed class LogsController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public LogsController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Route("latest")]
        [HttpGet]
        public async Task<List<Log>> GetAllUsers() =>
            await _guardRailContext
                .Logs
                .OrderByDescending(x => x.DateTime)
                .Take(100)
                .ToListAsync();
    }
}