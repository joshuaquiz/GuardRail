using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.DataModels;
using GuardRail.Core.Enums;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Services
{
    public sealed class LockService : IHostedService
    {
        private readonly IUdpReceiver _udpReceiver;
        private readonly IBuzzerManager _buzzerManager;
        private readonly ILightManager _lightManager;
        private readonly ILogger<LockService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _listener;

        public LockService(
            IUdpReceiver udpReceiver,
            IBuzzerManager buzzerManager,
            ILightManager lightManager,
            ILogger<LockService> logger)
        {
            _udpReceiver = udpReceiver;
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
                            var doorCommand = await _udpReceiver.ReceiveUdpMessage<DoorCommand>(cancellationToken);
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
                                tasks.Add(_buzzerManager.Buzz(doorCommand.BuzzerDuration.Value));
                            }

                            if (doorCommand.RedLightDuration.HasValue)
                            {
                                tasks.Add(_lightManager.TurnOnRedLight(doorCommand.RedLightDuration.Value));
                            }

                            if (doorCommand.GreenLightDuration.HasValue)
                            {
                                tasks.Add(_lightManager.TurnOnGreenLight(doorCommand.GreenLightDuration.Value));
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
}