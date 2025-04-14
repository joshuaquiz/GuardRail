using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Requests;
using GuardRail.Api.Models.Responses;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[UserAccessTokenAuthorization]
[Route("[controller]")]
public sealed class AccountController(
    IAccountManagementService accountManagementService,
    ILogger<AccountController> logger)
    : GhControllerBase<AccountController>(
        logger)
{
    [HttpPost(nameof(CreateAccount), Name = nameof(CreateAccount))]
    public async Task<ApiResult> CreateAccount(
        [FromBody]
        CreateNewAccountRequest createNewAccountRequest,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accountManagementService.CreateNewAccount(
                    createNewAccountRequest.AccountName,
                    createNewAccountRequest.FirstName,
                    createNewAccountRequest.LastName,
                    createNewAccountRequest.Phone,
                    createNewAccountRequest.Email,
                    cancellationToken));

    [HttpGet(nameof(GetDashboardData), Name = nameof(GetDashboardData))]
    public async Task<ApiResult<DashboardDataResponse>> GetDashboardData(
        [FromHeader]
        User user,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
            {
                var result = await accountManagementService.GetDashboardData(
                    user,
                    cancellationToken);
                return new DashboardDataResponse(
                    result.Accounts);
            });
}