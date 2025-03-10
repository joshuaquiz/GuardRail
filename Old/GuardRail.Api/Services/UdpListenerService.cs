using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Api.Services;

public sealed class UdpListenerService : IHostedService
{
    private readonly ILogger<UdpListenerService> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task _listener;

    public UdpListenerService(
        ILogger<UdpListenerService> logger)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        /*_listener = new TaskFactory()
            .StartNew(
                async () =>
                {
                    using var udpClient = new UdpClient(18989);
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        var receivedResults = await udpClient.ReceiveAsync(_cancellationTokenSource.Token);
                        var data = Encoding.ASCII.GetString(receivedResults.Buffer);
                        var type = data.Split(":~:")[0];
                        switch (type)
                        {
                            case nameof(UnLockRequest):
                                _eventBus.AccessAuthorizationEvents
                                    .Add(
                                        AccessAuthorizationEvent.Create(
                                            ,
                                            UdpAccessControlDevice.Create(receivedResults.RemoteEndPoint)));
                                break;
                        }
                    }
                },
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);*/
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        _listener.Dispose();
        return Task.CompletedTask;
    }
}