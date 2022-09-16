using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.DoorClient.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuardRail.DoorClient.Implementation;

public sealed class UdpWrapper : IUdpWrapper
{
    private static UdpClient _udpClient;

    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    private readonly ILogger<UdpWrapper> _logger;

    public UdpWrapper(ILogger<UdpWrapper> logger)
    {
        _logger = logger;
    }

    public void SetUdpClient(UdpClient udpClient)
    {
        _logger.LogDebug($"Setting the UdpClient to {(udpClient == null ? "null" : "a value")}");
        _udpClient = udpClient;
    }

    public async Task<UdpClient> GetUdpClient(CancellationToken cancellationToken)
    {
        if (_udpClient != null)
        {
            return _udpClient;
        }

        _logger.LogDebug("Waiting on the UdpClient");
        await _semaphoreSlim.WaitAsync(cancellationToken);
        while (_udpClient == null && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Still waiting on the UdpClient");
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        _semaphoreSlim.Release();
        _logger.LogDebug("Got the UdpClient");
        return _udpClient;

    }
}