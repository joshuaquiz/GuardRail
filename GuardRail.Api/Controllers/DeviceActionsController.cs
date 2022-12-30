using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Logic;
using GuardRail.Core.Data.Enums;
using GuardRail.Core.Data.Models;
using GuardRail.Core.EventModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuardRail.Api.Controllers;

[Authorize]
[Route("api/device_actions")]
[ApiController]
public sealed class DeviceActionsController : ControllerBase
{
    private readonly UserEntryRequestHandler _userEntryRequestHandler;
    private readonly IPushNotifications _pushNotifications;

    public DeviceActionsController(
        UserEntryRequestHandler userEntryRequestHandler,
        IPushNotifications pushNotifications)
    {
        _userEntryRequestHandler = userEntryRequestHandler;
        _pushNotifications = pushNotifications;
    }

    [Route(nameof(UnLockRequest))]
    [HttpGet]
    public async ValueTask<OkResult> UnLockRequestAsync(
        [FromBody] UnLockRequest unLockRequest,
        CancellationToken cancellationToken)
    {
        var shouldUnLockResult = await _userEntryRequestHandler.HandleUnLockRequest(unLockRequest, cancellationToken);
        if (shouldUnLockResult.ShouldUnLock)
        {
            await _pushNotifications.Send(
                new DoorCommand(
                    unLockRequest.AccessPointGuid,
                    new[]
                    {
                        new DoorStateRequest(
                            DoorStateRequestType.BuzzerOn,
                            TimeSpan.FromSeconds(1)),
                        new DoorStateRequest(
                            DoorStateRequestType.GreenLightOn,
                            TimeSpan.FromSeconds(3)),
                        new DoorStateRequest(
                            DoorStateRequestType.UnLocked,
                            TimeSpan.FromSeconds(3)),
                        new DoorStateRequest(
                            DoorStateRequestType.Open,
                            TimeSpan.FromSeconds(3))
                    },
                    $"{shouldUnLockResult.AccessPoint?.Name} has been unlocked and opened for 3 seconds by {shouldUnLockResult.User?.FirstName} {shouldUnLockResult.User?.LastName} as of {DateTime.UtcNow:O}"),
                cancellationToken);
        }

        return Ok();
    }
}