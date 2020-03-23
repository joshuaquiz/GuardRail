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
    [Microsoft.AspNetCore.Mvc.Route("api/users")]
    [ApiController]
    public sealed class UsersController : ControllerBase
    {
        private readonly GuardRailContext _guardRailContext;

        public UsersController(
            GuardRailContext guardRailContext)
        {
            _guardRailContext = guardRailContext;
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<List<UserModel>> GetAllUsers() =>
            await _guardRailContext
                .Users
                .Select(x =>
                new UserModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .ToListAsync();
    }
}