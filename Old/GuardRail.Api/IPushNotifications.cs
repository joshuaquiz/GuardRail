using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.EventModels;

namespace GuardRail.Api;

public interface IPushNotifications
{
    public ValueTask Send(
        DoorCommand doorCommand,
        CancellationToken cancellationToken);
}