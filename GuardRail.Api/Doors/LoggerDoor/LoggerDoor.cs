using System;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Api.Data;
using GuardRail.Core.Enums;

namespace GuardRail.Api.Doors.LoggerDoor;

public sealed class LoggerDoor : Door
{
    private static readonly Guid Id = Guid.NewGuid();

    private readonly GuardRailLogger _guardRailLogger;

    private bool _isLocked;

    public string FriendlyName { get; set; }

    public LoggerDoor(
        string friendlyName,
        GuardRailLogger guardRailLogger)
    {
        FriendlyName = friendlyName;
        _guardRailLogger = guardRailLogger;
    }

    public Task<string> GetDeviceId() =>
        Task.FromResult(Id.ToString());

    public Task<DoorStateRequestType> GetLockedStatus(CancellationToken cancellationToken) =>
        Task.FromResult(_isLocked
            ? DoorStateRequestType.Locked
            : DoorStateRequestType.UnLocked);

    public Task LockAsync(CancellationToken cancellationToken)
    {
        _isLocked = true;
        return Task.CompletedTask;
    }

    public async Task UnLockAsync(CancellationToken cancellationToken)
    {
        _isLocked = false;
        await _guardRailLogger.LogAsync($"{FriendlyName} is unlocked for 5 seconds");
        var delay = Task.Delay(
            TimeSpan.FromSeconds(5),
            cancellationToken);
        await delay;
        await LockAsync(CancellationToken.None);
    }
}