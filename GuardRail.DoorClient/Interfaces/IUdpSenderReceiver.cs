using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IUdpSenderReceiver
    {
        Task SendUdpMessageAsync<T>(T message, CancellationToken cancellationToken);

        Task<T> ReceiveUdpMessage<T>(CancellationToken cancellationToken);
    }
}