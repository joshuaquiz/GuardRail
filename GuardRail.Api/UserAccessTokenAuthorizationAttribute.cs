using System.Threading.Tasks;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GuardRail.Logic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Api;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UserAccessTokenAuthorizationAttribute
    : Attribute,
        IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var serviceProvider = context.HttpContext.RequestServices;
        var userManagementService = serviceProvider.GetRequiredService<IUserManagementService>();
        if (context.HttpContext
            .Request
            .Headers
            .TryGetValue(
                "x-auth-header",
                out var authHeaderValue))
        {
            if (Guid.TryParse(
                    authHeaderValue,
                    out var tokenGuid))
            {
                if (await userManagementService
                        .IsAuthTokenValid(
                            tokenGuid,
                            CancellationToken.None))
                {
                    return;
                }
            }
        }

        // Token is invalid or missing, return 401 Unauthorized
        context.Result = new UnauthorizedResult();
    }
}