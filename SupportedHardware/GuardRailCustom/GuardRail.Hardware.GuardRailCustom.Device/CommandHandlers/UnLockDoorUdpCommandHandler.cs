using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Door;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Buzzer;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces.Feedback.Lights;
using GuardRail.Logic.Commands.Models;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.CommandHandlers;

public sealed class UnLockDoorUdpCommandHandler(
    IDoorManager doorManager,
    IBuzzerManager buzzerManager,
    ILightManager lightManager,
    ILogger<UnLockDoorUdpCommandHandler> logger)
    : IUdpCommandHandler
{
    public static string CommandName =>
        GuardRailCustomConstants.UdpCommandNames.UnLockDoor;

    public async ValueTask<string?> HandleCommand(
        string commandBody,
        CancellationToken cancellationToken)
    {
        logger.LogGuardRailInformation($"Processing {CommandName}: {commandBody}");
        var unlockRequest = commandBody.FromJson<UnlockDoorCommandData>();
        if (unlockRequest == null)
        {
            return null;
        }

        var tasks = new List<Task>
        {
            doorManager.UnLockAsync(
                    unlockRequest.UnlockDuration,
                    cancellationToken)
                .AsTask()
        };
        if (unlockRequest.BuzzerDuration != null)
        {
            tasks.Add(
                buzzerManager.BuzzAsync(
                        unlockRequest.BuzzerDuration.Value,
                        cancellationToken)
                    .AsTask());
        }

        if (unlockRequest.GreenLightDuration != null)
        {
            tasks.Add(
                lightManager.TurnOnGreenLightAsync(
                        unlockRequest.GreenLightDuration.Value,
                        cancellationToken)
                    .AsTask());
        }

        await Task.WhenAll(
            tasks);
        return null;
    }
}