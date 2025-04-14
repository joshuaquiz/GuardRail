using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Responses;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[UserAccessTokenAuthorization]
[Route("[controller]")]
public sealed class VersionController(
    IVersionManagementService versionManagementService,
    ILogger<VersionController> logger)
    : GhControllerBase<VersionController>(
        logger)
{
    [HttpGet(Name = nameof(VersionCheck))]
    public async Task<ApiResult<VersionCheckResponse>> VersionCheck(
        [FromQuery]
        Version version,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
            {
                var result = await versionManagementService.VersionCheck(
                    version,
                    cancellationToken);
                return new VersionCheckResponse(
                    result.IsLatest,
                    result.IsUpdateRequired,
                    result.LatestVersion,
                    result.InstallFiles);
            });
}