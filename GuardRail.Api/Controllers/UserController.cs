using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Requests;
using GuardRail.Core.Exceptions;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController(
    IUserManagementService userManagementService,
    ILogger<UserController> logger)
    : GhControllerBase<UserController>(
        logger)
{
    [HttpPost(nameof(SignIn), Name = nameof(SignIn))]
    public async Task<ApiResult<Guid>> SignIn(
        [FromQuery]
        string email,
        [FromBody]
        string password,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await userManagementService.SignIn(
                    email,
                    password,
                    cancellationToken),
            new Dictionary<Type, Func<Exception, ILogger<UserController>, ApiResult<Guid>>>
            {
                {
                    typeof(UserNotAllowedToSignInException),
                    (e, logger) =>
                    {
                        var ex = (e as UserNotAllowedToSignInException)!;
                        logger.LogError(
                            e,
                            ex.Message);
                        return Unauthorized<Guid>(
                            ex.ExternalMessage);
                    }
                }
            });

    [HttpPost(nameof(ResetPassword), Name = nameof(ResetPassword))]
    public async Task<ApiResult> ResetPassword(
        [FromQuery]
        Guid userId,
        [FromBody]
        string password,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await userManagementService.ResetPassword(
                    userId,
                    password,
                    cancellationToken),
            new Dictionary<Type, Func<Exception, ILogger<UserController>, ApiResult>>
            {
                {
                    typeof(UserCouldNotBeFoundException),
                    (e, logger) =>
                    {
                        var ex = (e as UserCouldNotBeFoundException)!;
                        logger.LogError(
                            e,
                            ex.Message);
                        return Error(
                            ex.ExternalMessage);
                    }
                }
            });

    [HttpPost(nameof(CreateNewUser), Name = nameof(CreateNewUser))]
    [UserAccessTokenAuthorization]
    public async Task<ApiResult> CreateNewUser(
        [FromBody]
        CreateNewUserRequest createNewUserRequest,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await userManagementService.CreateNewUser(
                    createNewUserRequest.AccountId,
                    createNewUserRequest.FirstName,
                    createNewUserRequest.LastName,
                    createNewUserRequest.Phone,
                    createNewUserRequest.Email,
                    createNewUserRequest.UserType,
                    cancellationToken));

    [HttpGet(nameof(ListUsers), Name = nameof(ListUsers))]
    [UserAccessTokenAuthorization]
    public async Task<ApiResult<IReadOnlyCollection<User>>> ListUsers(
        [FromQuery]
        Guid accountId,
        [FromQuery]
        IReadOnlyCollection<string>? tags,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await userManagementService.ListUsers(
                    accountId,
                    tags,
                    cancellationToken));
}