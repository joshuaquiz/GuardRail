using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GuardRail.Core.Data;
using GuardRail.Core.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GuardRail.Core.Helpers;

namespace GuardRail.Api;

public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string InvalidAuthorizationHeaderErrorMessage = "Invalid Authorization Header";

    private readonly GuardRailContext _guardRailContext;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        GuardRailContext guardRailContext)
        : base(options, logger, encoder, clock)
    {
        _guardRailContext = guardRailContext;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        User? user;
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Parameter.IsNullOrWhiteSpace())
            {
                return AuthenticateResult.Fail(InvalidAuthorizationHeaderErrorMessage);
            }

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var accessKeyString = Encoding.UTF8.GetString(credentialBytes).Replace("Bearer ", string.Empty);
            var accessKey = Guid.Parse(accessKeyString);
            var key = await _guardRailContext.ApiAccessKeys.SingleOrDefaultAsync(x => x.Guid == accessKey && x.Expiry > DateTimeOffset.UtcNow);
            user = key?.User;
        }
        catch
        {
            return AuthenticateResult.Fail(InvalidAuthorizationHeaderErrorMessage);
        }

        if (user == null)
        {
            return AuthenticateResult.Fail(InvalidAuthorizationHeaderErrorMessage);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}