using System;
using System.Collections.Generic;
using System.Linq;
using GuardRail.Core.Enums;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Door;
using GuardRail.Core.EventModels;
using GuardRail.DeviceLogic.Interfaces.Feedback;

namespace GuardRail.DeviceLogic.Logic;

public static class DoorCommandEventHandlers
{
    public static ValueTask ConfigureCoreDoorCommandEventHandler(
        ICentralServerPushCommunication centralServerPushCommunication,
        IBuzzerManager buzzerManager,
        ILightManager lightManager,
        IDoorManager doorManager,
        IScreenManager screenManager,
        ILogger<DoorCommand> logger) =>
        centralServerPushCommunication.ConfigureDataReceiverAsync<DoorCommand>(async (doorCommand, cancellationToken) =>
            {
                if (doorCommand == null)
                {
                    return;
                }

                var valueTasks = new List<ValueTask>();
                if (!string.IsNullOrEmpty(doorCommand.Message))
                {
                    valueTasks.Add(
                        screenManager.DisplayMessageAsync(
                            doorCommand.Message,
                            TimeSpan.FromSeconds(3),
                            cancellationToken));
                }

                foreach (var doorCommandDoorStateRequest in doorCommand.DoorStateRequests ?? Array.Empty<DoorStateRequest>())
                {
                    switch (doorCommandDoorStateRequest.DoorStateRequestType)
                    {
                        case DoorStateRequestType.UnLocked:
                            valueTasks.Add(
                                doorManager.UnLockAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.Locked:
                            valueTasks.Add(
                                doorManager.LockAsync(
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.Open:
                            valueTasks.Add(
                                doorManager.OpenAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.Closed:
                            valueTasks.Add(
                                doorManager.CloseAsync(
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.GreenLightOn:
                            valueTasks.Add(
                                lightManager.TurnOnGreenLightAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.RedLightOn:
                            valueTasks.Add(
                                lightManager.TurnOnRedLightAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.BuzzerOn:
                            valueTasks.Add(
                                buzzerManager.BuzzAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            break;
                        case DoorStateRequestType.UnKnown:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                while (valueTasks.Any(x => !x.IsCompleted))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
            });
}