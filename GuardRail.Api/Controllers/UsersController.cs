using System.Collections.Generic;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public sealed class UsersController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public UsersController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Route("")]
        [HttpGet]
        public async Task<List<User>> GetAllUsers() =>
            await _guardRailContext.Users.ToListAsync();
    }
}