using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Core.Models;
using Microsoft.Extensions.Hosting;

namespace GuardRail.LocalClient;

/// <summary>
/// Listens for devices on the network.
/// </summary>
public sealed class NdsService : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Creates a new <see cref="NdsService"/>
    /// </summary>
    public NdsService()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) =>
        new TaskFactory()
            .StartNew(async () =>
                {
                    var server = new UdpClient(18989);
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var clientRequestData = await server.ReceiveAsync(cancellationToken);
                        var ping = Encoding.UTF8.GetString(clientRequestData.Buffer).FromJson<NdsPing>();
                        if (ping?.RequestId != Guid.Empty)
                        {
                            await server.SendAsync(clientRequestData.Buffer, clientRequestData.Buffer.Length, clientRequestData.RemoteEndPoint);
                        }
                    }
                },
                _cancellationTokenSource.Token);

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}