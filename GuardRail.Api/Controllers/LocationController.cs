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
public sealed class LocationController(
    ILocationManagementService locationManagementService,
    ILogger<LocationController> logger)
    : GhControllerBase<LocationController>(
        logger)
{
    [HttpPost(Name = nameof(CreateLocation))]
    public async Task<ApiResult> CreateLocation(
        [FromBody]
        CreateNewLocationRequest createNewLocationRequest,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await locationManagementService.CreateNewLocation(
                    createNewLocationRequest.AccountId,
                    createNewLocationRequest.IsMobile,
                    createNewLocationRequest.Name,
                    createNewLocationRequest.Description,
                    createNewLocationRequest.Latitude,
                    createNewLocationRequest.Longitude,
                    createNewLocationRequest.GeoFenceDistance,
                    cancellationToken));

    [HttpPost(Name = nameof(SetCommunicationMethod))]
    public async Task<ApiResult> SetCommunicationMethod(
        [FromQuery]
        Guid locationId,
        [FromQuery]
        LocationCallbackType locationCallbackType,
        [FromBody]
        Uri? uri,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await locationManagementService.SetCommunicationMethod(
                    locationId,
                    locationCallbackType,
                    uri,
                    cancellationToken));

    [HttpGet(Name = nameof(ListLocations))]
    public async Task<ApiResult<IReadOnlyCollection<Location>>> ListLocations(
        [FromQuery]
        Guid accountId,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await locationManagementService.ListLocations(
                    accountId,
                    cancellationToken));
}