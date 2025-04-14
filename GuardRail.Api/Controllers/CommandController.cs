using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[UserAccessTokenAuthorization]
[Route("[controller]")]
public sealed class CommandController(
    ICommandManagementService commandManagementService,
    ILogger<CommandController> logger)
    : GhControllerBase<CommandController>(
        logger)
{
    [HttpGet(nameof(ListCommands), Name = nameof(ListCommands))]
    public async Task<ApiResult> ListCommands(
        [FromQuery]
        Guid locationId,
        [FromQuery]
        CommandStatus? status,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await commandManagementService.ListCommands(
                    locationId,
                    status,
                    cancellationToken));

    [HttpGet(nameof(ListPendingCommands), Name = nameof(ListPendingCommands))]
    public async Task<ApiResult> ListPendingCommands(
        [FromQuery]
        Guid accountId,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await commandManagementService.ListCommands(
                    accountId,
                    CommandStatus.Pending,
                    cancellationToken));

    [HttpPost(nameof(CreateCommand), Name = nameof(CreateCommand))]
    public async Task<ApiResult<Guid>> CreateCommand(
        [FromQuery]
        Guid locationId,
        [FromQuery]
        CommandType type,
        [FromQuery]
        DateTimeOffset expiryDate,
        [FromQuery]
        int? maxRetries,
        [FromBody]
        string body,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
            {
                var command = await commandManagementService.AddNewCommand(
                    locationId,
                    type,
                    expiryDate,
                    maxRetries,
                    body,
                    cancellationToken);
                return command.Guid;
            });

    [HttpPost(nameof(UpdateCommand), Name = nameof(UpdateCommand))]
    public async Task<ApiResult> UpdateCommand(
        [FromQuery]
        Guid commandId,
        [FromQuery]
        CommandStatus status,
        [FromBody]
        string? response,
        CancellationToken cancellationToken) =>
        await GhWrappedApiCall(
            async () =>
                await commandManagementService.UpdateCommand(
                    commandId,
                    status,
                    response,
                    cancellationToken));
}