using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Models.Requests;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[UserAccessTokenAuthorization]
[Route("[controller]")]
public sealed class AccessPointController(
    IAccessPointManagementService accessPointManagementService,
    ILogger<AccessPointController> logger)
    : GhControllerBase<AccessPointController>(
        logger)
{
    [HttpPost(nameof(CreateAccessPoint), Name = nameof(CreateAccessPoint))]
    public async Task<ApiResult> CreateAccessPoint(
        [FromBody]
        CreateNewAccessPointRequest createNewAccessPointRequest,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accessPointManagementService.CreateNewAccessPoint(
                    createNewAccessPointRequest.LocationGuid,
                    createNewAccessPointRequest.Name,
                    createNewAccessPointRequest.AccessPointType,
                    createNewAccessPointRequest.Latitude,
                    createNewAccessPointRequest.Longitude,
                    createNewAccessPointRequest.GeoFenceDistance,
                    createNewAccessPointRequest.RequiresAllAccessMethods,
                    createNewAccessPointRequest.AccessMethodTimeout,
                    cancellationToken));

    [HttpGet(nameof(ListAccessPoints), Name = nameof(ListAccessPoints))]
    public async Task<ApiResult<IReadOnlyCollection<AccessPoint>>> ListAccessPoints(
        [FromQuery]
        Guid locationId,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accessPointManagementService.ListAccessPoints(
                    locationId,
                    cancellationToken));

    [HttpGet(nameof(GetAvailableAccessPoints), Name = nameof(GetAvailableAccessPoints))]
    public async Task<ApiResult<IReadOnlyCollection<string>>> GetAvailableAccessPoints(
        [FromQuery]
        Guid locationId,
        [FromQuery]
        AccessPointType accessPointType,
        [FromQuery]
        TimeSpan timeout,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accessPointManagementService.GetAvailableAccessPoints(
                    locationId,
                    accessPointType,
                    timeout,
                    cancellationToken));
}