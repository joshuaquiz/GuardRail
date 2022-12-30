using System;
using System.Collections.Generic;
using System.Linq;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GuardRail.Core.Data.Enums;
using GuardRail.DeviceLogic.Interfaces.Door;
using GuardRail.Core.EventModels;
using GuardRail.DeviceLogic.Interfaces.Feedback;

namespace GuardRail.DeviceLogic.Logic;

public static class DoorCommandEventHandlers
{
    public static ValueTask ConfigureCoreDoorCommandEventHandler(
        ICentralServerPushCommunication centralServerPushCommunication,
        IBuzzerManager? buzzerManager,
        ILightManager? lightManager,
        IDoorManager? doorManager,
        IScreenManager? screenManager,
        ILogger<DoorCommand> logger) =>
        centralServerPushCommunication.ConfigureDataReceiverAsync<DoorCommand>(async (doorCommand, cancellationToken) =>
            {
                if (doorCommand == null)
                {
                    return;
                }

                var valueTasks = new List<ValueTask>();
                if (screenManager is not null && !string.IsNullOrEmpty(doorCommand.Message))
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
                            if (doorManager is not null)
                            {
                                valueTasks.Add(
                                    doorManager.UnLockAsync(
                                        doorCommandDoorStateRequest.Duration,
                                        cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.Locked:
                            if (doorManager is not null)
                            {
                                valueTasks.Add(
                                doorManager.LockAsync(
                                    cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.Open:
                            if (doorManager is not null)
                            {
                                valueTasks.Add(
                                doorManager.OpenAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.Closed:
                            if (doorManager is not null)
                            {
                                valueTasks.Add(
                                doorManager.CloseAsync(
                                    cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.GreenLightOn:
                            if (lightManager is not null)
                            {
                                valueTasks.Add(
                                lightManager.TurnOnGreenLightAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.RedLightOn:
                            if (lightManager is not null)
                            {
                                valueTasks.Add(
                                lightManager.TurnOnRedLightAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            }
                            break;
                        case DoorStateRequestType.BuzzerOn:
                            if (buzzerManager is not null)
                            {
                                valueTasks.Add(
                                buzzerManager.BuzzAsync(
                                    doorCommandDoorStateRequest.Duration,
                                    cancellationToken));
                            }
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