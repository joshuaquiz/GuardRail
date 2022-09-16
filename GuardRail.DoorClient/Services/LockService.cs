using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services;

public sealed class LockService : IHostedService
{
    private readonly IUdpSenderReceiver _udpSenderReceiver;
    private readonly IBuzzerManager _buzzerManager;
    private readonly ILightManager _lightManager;
    private readonly ILogger<LockService> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private Task _listener;

    public LockService(
        IUdpSenderReceiver udpSenderReceiver,
        IBuzzerManager buzzerManager,
        ILightManager lightManager,
        ILogger<LockService> logger)
    {
        _udpSenderReceiver = udpSenderReceiver;
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
                        var doorCommand = await _udpSenderReceiver.ReceiveUdpMessage<DoorCommand>(cancellationToken);
                        var tasks = new List<Task>();
                        switch (doorCommand.LockedStatus)
                        {
                            case LockedStatus.Locked:
                                _logger.LogInformation("Locking door");
                                break;
                            case LockedStatus.UnLocked:
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