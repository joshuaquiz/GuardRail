using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface ILightManager
    {
        Task TurnOnRedLightAsync(TimeSpan duration, CancellationToken cancellationToken);

        Task TurnOnGreenLightAsync(TimeSpan duration, CancellationToken cancellationToken);
    }
}