using System.Linq;
using GuardRail.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using GuardRail.Core.DataModels;
using Door = GuardRail.Api.Data.Door;
using User = GuardRail.Api.Data.User;

namespace GuardRail.Api.Logic;

public sealed class UserEntryRequestHandler
{
    private readonly GuardRailContext _guardRailContext;

    public UserEntryRequestHandler(
        GuardRailContext guardRailContext)
    {
        _guardRailContext = guardRailContext;
    }

    public async ValueTask<UnLockResult> HandleUnLockRequest(
        UnLockRequest unLockRequest,
        CancellationToken cancellationToken)
    {
        var door = await _guardRailContext.Doors.FirstOrDefaultAsync(x => x.DeviceId == unLockRequest.DoorId.ToString(), cancellationToken);
        if (door is null)
        {
            return new UnLockResult(null, null, false);
        }

        var user = (await _guardRailContext.UserAccessMethods
                .FirstOrDefaultAsync(x =>
                        x.UnlockRequestType == unLockRequest.UnlockRequestType
                        && x.Data == unLockRequest.Data,
                    cancellationToken))
            ?.User;
        if (user is null)
        {
            return new UnLockResult(null, null, false);
        }

        var doesUserHaveAccessToDoor = user.DoorUserAccesses.Any(x => x.Door.Id == door.Id);
        return new UnLockResult(user, door, doesUserHaveAccessToDoor);
    }
}

public sealed record UnLockResult(
    User? User,
    Door? Door,
    bool ShouldUnLock);