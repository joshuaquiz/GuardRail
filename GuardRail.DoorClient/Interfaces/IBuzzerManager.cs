using System;
using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IBuzzerManager
    {
        Task BuzzAsync(TimeSpan duration, CancellationToken cancellationToken);
    }
}