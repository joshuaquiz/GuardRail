using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.DoorClient.Interfaces
{
    public interface IUdpSender
    {
        Task SendUdpMessageAsync<T>(T message, CancellationToken cancellationToken);
    }
}