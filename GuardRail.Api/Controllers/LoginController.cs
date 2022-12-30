using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models;
using GuardRail.Core.Data;
using GuardRail.Core.Data.Models;
using GuardRail.Core.Helpers;
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
                && x.Password == loginModel.Password.GetBytes(),
            cancellationToken);
        if (user == null)
        {
            return new LoginResponseModel(
                false,
                null,
                null,
                null,
                false);
        }

        var needsPasswordReset = user.PasswordResetDate < DateOnly.FromDateTime(DateTime.UtcNow);
        if (!needsPasswordReset)
        {
            await _guardRailContext.ApiAccessKeys.AddAsync(
                new ApiAccessKey
                {
                    Guid = Guid.NewGuid(),
                    AccountGuid = ApplicationGlobals.Account.Guid,
                    User = user,
                    UserGuid = user.Guid,
                    Expiry = DateTimeOffset.UtcNow.AddDays(24)
                },
                cancellationToken);
            await _guardRailContext.SaveChangesAsync(cancellationToken);
        }

        return new LoginResponseModel(
            !needsPasswordReset,
            user.Email,
            $"{user.FirstName} {user.LastName}",
            Guid.NewGuid(),
            needsPasswordReset);
    }
}