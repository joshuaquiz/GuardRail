using GuardRail.Core.Enums;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Interfaces.Door;
using GuardRail.Core.EventModels;

namespace GuardRail.DeviceLogic.Logic;

public static class DoorCommandEventHandlers
{
    public static ValueTask ConfigureDoorCommandEventHandlers(
        ICentralServerPushCommunication centralServerPushCommunication,
        IBuzzerManager buzzerManager,
        ILightManager lightManager,
        IDoorManager doorManager,
        ILogger<DoorCommand> logger)
    {
        return centralServerPushCommunication.ConfigureDataReceiverAsync<DoorCommand>(
            async (doorCommand, cancellationToken) =>
            {
                if (doorCommand == null)
                {
                    return;
                }

                switch (doorCommand.DoorStatus)
                {
                    case DoorStatus.Locked:
                        await doorManager.LockAsync(cancellationToken);
                        break;
                    case DoorStatus.UnLocked:
                        await doorManager.UnLockAsync(
                            doorCommand.BuzzerDuration,
                            cancellationToken);
                        break;
                    case DoorStatus.UnLocked:
                        logger.LogInformation("Unlocking door");
                        break;
                    case DoorStatus.UnKnown:
                    default:
                        logger.LogInformation("IDK my BFF Jill?!?!");
                        break;
                }

                if (doorCommand.BuzzerDuration.HasValue)
                {
                    tasks.Add(buzzerManager.BuzzAsync(doorCommand.BuzzerDuration.Value, cancellationToken));
                }

                if (doorCommand.RedLightDuration.HasValue)
                {
                    tasks.Add(lightManager.TurnOnRedLightAsync(doorCommand.RedLightDuration.Value, cancellationToken));
                }

                if (doorCommand.GreenLightDuration.HasValue)
                {
                    tasks.Add(lightManager.TurnOnGreenLightAsync(doorCommand.GreenLightDuration.Value, cancellationToken));
                }
            });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener = new TaskFactory()
            .StartNew(
                async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var doorCommand = await _centralServerCommunication.ReceiveUdpMessage<DoorCommand>(cancellationToken);
                        var tasks = new List<ValueTask>();
                        switch (doorCommand.LockedStatus)
                        {
                            case DoorStatus.Locked:
                                _logger.LogInformation("Locking door");
                                break;
                            case DoorStatus.UnLocked:
                                _logger.LogInformation("Unlocking door");
                                break;
                            default:
                                _logger.LogInformation("IDK my BFF Jill?!?!");
                                break;
                        }

                        if (doorCommand.BuzzerDuration.HasValue)
                        {
                            tasks.Add(_buzzerManager.BuzzAsync(doorCommand.BuzzerDuration.Value, cancellationToken));
                        }

                        if (doorCommand.RedLightDuration.HasValue)
                        {
                            tasks.Add(_lightManager.TurnOnRedLightAsync(doorCommand.RedLightDuration.Value, cancellationToken));
                        }

                        if (doorCommand.GreenLightDuration.HasValue)
                        {
                            tasks.Add(_lightManager.TurnOnGreenLightAsync(doorCommand.GreenLightDuration.Value, cancellationToken));
                        }

                        await Task.WhenAll(tasks);
                    }
                },
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        _listener.Dispose();
        return Task.CompletedTask;
    }
}