using GuardRail.Api.Models;
using GuardRail.Api.Models.Requests;
using GuardRail.Core.Enums;
using GuardRail.Core.Models.Models;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuardRail.Api.Main.Controllers;

[ApiController]
[Route("[controller]")]
public class AccessPointController(
    IAccessPointManagementService accessPointManagementService,
    ILogger<AccessPointController> logger)
    : GhControllerBase<AccessPointController>(
        logger)
{
    [HttpPost(Name = nameof(CreateAccessPoint))]
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

    [HttpGet(Name = nameof(ListAccessPoints))]
    public async Task<ApiResult<IReadOnlyCollection<AccessPoint>>> ListAccessPoints(
        [FromQuery]
        Guid locationId,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accessPointManagementService.ListAccessPoints(
                    locationId,
                    cancellationToken));

    [HttpGet(Name = nameof(GetAvailableAccessPoints))]
    public async Task<ApiResult<IReadOnlyCollection<AccessPoint>>> GetAvailableAccessPoints(
        [FromQuery]
        Guid locationId,
        [FromQuery]
        AccessPointType accessPointType,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await accessPointManagementService.GetAvailableAccessPoints(
                    locationId,
                    accessPointType,
                    cancellationToken));
}