using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Controllers;

[AllowAnonymous]
[Route("api/login")]
[ApiController]
public sealed class LoginController : ControllerBase
{
    private readonly GuardRailContext _guardRailContext;

    public LoginController(
        GuardRailContext guardRailContext)
    {
        _guardRailContext = guardRailContext;
    }

    [HttpPost]
    public async Task<LoginResponseModel> LoginAsync(
        LoginModel loginModel,
        CancellationToken cancellationToken)
    {
        if (!await _guardRailContext.Users.AnyAsync(cancellationToken))
        {
            return new LoginResponseModel(
                false,
                loginModel.Username,
                "Admin",
                null,
                true);
        }

        var user = await _guardRailContext.Users.SingleOrDefaultAsync(
            x =>
                x.Username == loginModel.Username
                && x.Password == loginModel.Password,
            cancellationToken);
        return new LoginResponseModel(
            user != null,
            user?.Email,
            user?.FirstName + " " + user?.LastName,
            Guid.NewGuid(),
            false);
    }
}