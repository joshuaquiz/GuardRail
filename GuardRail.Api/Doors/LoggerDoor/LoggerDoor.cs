using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using Serilog;

namespace GuardRail.Api.Doors.LoggerDoor
{
    public sealed class LoggerDoor : IDoor
    {
        private static readonly Guid Id = Guid.NewGuid();

        private readonly ILogger _logger;

        private bool _isLocked;

        public LoggerDoor(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task<string> GetDeviceId() =>
            Task.FromResult(Id.ToString());

        public Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken) =>
            Task.FromResult(_isLocked
                ? LockedStatus.Locked
                : LockedStatus.UnLocked);

        public Task Lock(CancellationToken cancellationToken)
        {
            _logger.Debug("Locking door");
            return Task.CompletedTask;
        }

        public Task UnLock(CancellationToken cancellationToken)
        {
            _logger.Debug("Unlocking door");
            return Task.CompletedTask;
        }
    }
}