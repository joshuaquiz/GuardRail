using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;
using Serilog;

namespace GuardRail.Api.Doors.ConsoleLoggerDoor
{
    public sealed class ConsoleLoggerDoorFactory : IDoorFactory
    {
        private readonly ILogger _logger;

        public ConsoleLoggerDoorFactory(
            ILogger logger)
        {
            _logger = logger;
        }

        public IReadOnlyCollection<IDoor> GetDoors()
        {
            return new ReadOnlyCollection<IDoor>(
                new List<IDoor>
                {
                    new ConsoleLoggerDoor(_logger)
                });
        }
    }

    public sealed class ConsoleLoggerDoor : IDoor
    {
        private readonly ILogger _logger;

        public bool _isLocked;

        public ConsoleLoggerDoor(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken)
        {
            return Task.FromResult(_isLocked
                ? LockedStatus.Locked
                : LockedStatus.UnLocked);
        }

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