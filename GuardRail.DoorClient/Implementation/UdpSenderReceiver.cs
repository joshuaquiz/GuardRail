using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DoorClient.Interfaces;

namespace GuardRail.DoorClient.Implementation
{
    public sealed class UdpSenderReceiver : IUdpSender, IUdpReceiver
    {
        private readonly UdpClient _udpClient;

        public UdpSenderReceiver(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public Task SendUdpMessageAsync<T>(T message, CancellationToken cancellationToken)
        {
            var receiveDataTask = new Task(
                async () =>
                {
                    var data = Encoding.UTF8.GetBytes(message.ToJson());
                    await _udpClient.SendAsync(data, data.Length);
                },
                cancellationToken);
            receiveDataTask.Start();
            return receiveDataTask;
        }

        public async Task<T> ReceiveUdpMessage<T>(CancellationToken cancellationToken)
        {
            UdpReceiveResult data = default;
            var receiveDataTask = new Task(async () => data = await _udpClient.ReceiveAsync(), cancellationToken);
            await receiveDataTask;
            cancellationToken.ThrowIfCancellationRequested();
            return Encoding.UTF8.GetString(data.Buffer).FromJson<T>();
        }
    }
}