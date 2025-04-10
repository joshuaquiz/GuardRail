using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CommandController(
    ICommandManagementService commandManagementService,
    ILogger<CommandController> logger)
    : GhControllerBase<CommandController>(
        logger)
{
    [HttpPost(Name = nameof(CreateCommand))]
    public async Task<ApiResult<Guid>> CreateCommand(
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
                    type,
                    expiryDate,
                    maxRetries,
                    body,
                    cancellationToken);
                return command.Guid;
            });

    [HttpPost(Name = nameof(UpdateCommand))]
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