using GuardRail.Core.Data.Models;

namespace GuardRail.Api.Models;

public sealed record UnLockResult(
    User? User,
    AccessPoint? AccessPoint,
    bool ShouldUnLock);