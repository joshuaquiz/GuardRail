using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IUdpReceiver
    {
        Task<T> ReceiveUdpMessage<T>(CancellationToken cancellationToken);
    }
}