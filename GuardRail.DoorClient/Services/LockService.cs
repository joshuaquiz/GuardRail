using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Core.EventModels;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services;

public sealed class LockService : IHostedService
{
    private readonly ICentralServerCommunication _centralServerCommunication;
    private readonly IBuzzerManager _buzzerManager;
    private readonly ILightManager _lightManager;
    private readonly ILogger<LockService> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private Task _listener;

    public LockService(
        ICentralServerCommunication centralServerCommunication,
        IBuzzerManager buzzerManager,
        ILightManager lightManager,
        ILogger<LockService> logger)
    {
        _centralServerCommunication = centralServerCommunication;
        _buzzerManager = buzzerManager;
        _lightManager = lightManager;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
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