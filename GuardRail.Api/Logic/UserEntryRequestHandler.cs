using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using GuardRail.Core.Data;
using GuardRail.Core.Data.Models;
using GuardRail.Api.Models;

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
        var accessMethod = await _guardRailContext.UserAccessMethods
            .FirstOrDefaultAsync(
                x =>
                    x.UnlockRequestType == unLockRequest.UnlockRequestType
                    && x.Data == unLockRequest.Data,
                cancellationToken);
        if (accessMethod is null)
        {
            return new UnLockResult(null, null, false);
        }

        var accessPoint = await _guardRailContext.AccessPoints
            .FirstOrDefaultAsync(
                x => x.Guid == unLockRequest.AccessPointGuid,
                cancellationToken);
        if (accessPoint is null)
        {
            return new UnLockResult(null, null, false);
        }

        var grantedAccess = await _guardRailContext.GrantedAccesses
            .Where(x =>
                (GrantedAccessesContainsAccessPointDirectly(x, accessPoint)
                 || GrantedAccessesContainsAccessPointInAccessPointGroup(x, accessPoint))
                && (GrantedAccessesContainsUserDirectly(x, accessMethod.User)
                    || GrantedAccessesContainsUserInUserGroup(x, accessMethod.User)))
            .ToListAsync(cancellationToken);
        return RestrictionsAllowAccess(grantedAccess, accessMethod)
            ? new UnLockResult(accessMethod.User, accessPoint, true)
            : new UnLockResult(null, null, false);
    }

    private static bool GrantedAccessesContainsAccessPointDirectly(
        GrantedAccess grantedAccess,
        AccessPoint accessPoint) =>
        grantedAccess.AccessPoints != null
        && grantedAccess.AccessPoints.Contains(accessPoint);

    private static bool GrantedAccessesContainsAccessPointInAccessPointGroup(
        GrantedAccess grantedAccess,
        AccessPoint accessPoint) =>
        grantedAccess.AccessPointGroups != null
        && grantedAccess.AccessPointGroups.Any(x => x.AccessPoints.Contains(accessPoint));

    private static bool GrantedAccessesContainsUserDirectly(
        GrantedAccess grantedAccess,
        User user) =>
        grantedAccess.User != null
        && grantedAccess.User == user;

    private static bool GrantedAccessesContainsUserInUserGroup(
        GrantedAccess grantedAccess,
        User user) =>
        grantedAccess.UserGroup != null
        && grantedAccess.UserGroup.Users.Contains(user);

    private static bool RestrictionsAllowAccess(
        IEnumerable<GrantedAccess> grantedAccess,
        UserAccessMethod accessMethod)
    {
        var allowed = true;
        foreach (var restriction in grantedAccess
                     .Where(x =>
                         x.Restrictions != null
                         && x.Restrictions.Any())
                     .SelectMany(x => x.Restrictions!))
        {
            if (restriction.StartTime is not null
                && restriction.StartTime > TimeOnly.FromDateTime(DateTime.UtcNow))
            {
                allowed = false;
                break;
            }

            if (restriction.EndTime is not null
                && restriction.EndTime < TimeOnly.FromDateTime(DateTime.UtcNow))
            {
                allowed = false;
                break;
            }

            if (restriction.StartDate is not null
                && restriction.StartDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                allowed = false;
                break;
            }

            if (restriction.EndDate is not null
                && restriction.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                allowed = false;
                break;
            }

            if (restriction.AllowedUnlockRequestTypes is not null
                && !restriction.AllowedUnlockRequestTypes.Contains(accessMethod.UnlockRequestType))
            {
                allowed = false;
                break;
            }

            if (restriction.BlockedUnlockRequestTypes is not null
                && restriction.BlockedUnlockRequestTypes.Contains(accessMethod.UnlockRequestType))
            {
                allowed = false;
                break;
            }
        }

        return allowed;
    }
}