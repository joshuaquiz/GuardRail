using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Api.Models;
using GuardRail.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers;

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
    public async Task<List<UserModel>> GetAllUsers() =>
        await _guardRailContext
            .Users
            .Select(x =>
                new UserModel
                {
                    Id = x.Guid,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
            .ToListAsync();
}