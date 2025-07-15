using GuardRail.Core.Models;

namespace GuardRail.Hardware.Common;

/// <summary>
/// This interface is used to provide access request functionality for hardware implementations.
/// </summary>
/// <remarks>
/// The implementation of this interface is handled in the local service.
/// </remarks>
public interface IAccessRequestProvider
{
    public Task TriggerRequestAccess(
        UnlockRequestCommandData unlockRequest,
        CancellationToken cancellationToken);
}