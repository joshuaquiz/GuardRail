using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core;

namespace GuardRail.Api.Doors.LoggerDoor
{
    public sealed class LoggerDoor : IDoor
    {
        private static readonly Guid Id = Guid.NewGuid();

        private bool _isLocked;

        public Task<string> GetDeviceId() =>
            Task.FromResult(Id.ToString());

        public Task<LockedStatus> GetLockedStatus(CancellationToken cancellationToken) =>
            Task.FromResult(_isLocked
                ? LockedStatus.Locked
                : LockedStatus.UnLocked);

        public Task Lock(CancellationToken cancellationToken)
        {
            _isLocked = true;
            return Task.CompletedTask;
        }

        public Task UnLock(CancellationToken cancellationToken)
        {
            _isLocked = false;
            return Task.CompletedTask;
        }
    }
}