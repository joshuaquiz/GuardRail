using System.Collections.Generic;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers
{
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
        public Task<List<User>> GetAllUsers() =>
            _guardRailContext.Users.ToListAsync();
    }
}