using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.DoorClient.Interfaces;
using Polly;
using Polly.Retry;

namespace GuardRail.DoorClient.Implementation
{
    public sealed class UdpSenderReceiverReceiver : IUdpSenderReceiver
    {
        private readonly IUdpWrapper _udpClient;
        private readonly AsyncRetryPolicy _policy;

        public UdpSenderReceiverReceiver(IUdpWrapper udpClient)
        {
            _udpClient = udpClient;
            var jitter = new Random();
            _policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    2,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)));
        }

        public async Task SendUdpMessageAsync<T>(T message, CancellationToken cancellationToken) =>
            await _policy.ExecuteAsync(
                async () =>
                {
                    var client = await _udpClient.GetUdpClient(cancellationToken);
                    var data = Encoding.UTF8.GetBytes(message.ToJson());
                    await client.SendAsync(data, data.Length);
                });

        public async Task<T> ReceiveUdpMessage<T>(CancellationToken cancellationToken) =>
            await _policy.ExecuteAsync(
                async () =>
                {
                    var client = await _udpClient.GetUdpClient(cancellationToken);
                    var data = await client.ReceiveAsync();
                    return Encoding.UTF8.GetString(data.Buffer).FromJson<T>();
                });
    }
}