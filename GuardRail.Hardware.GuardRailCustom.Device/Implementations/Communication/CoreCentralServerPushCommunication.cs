using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device.Implementations.Communication;

public abstract class CoreCentralServerPushCommunication<T>(ILogger<T> logger) : ICentralServerPushCommunication
    where T : CoreCentralServerPushCommunication<T>
{
    protected readonly ILogger<T> Logger = logger;
    protected readonly IDictionary<string, IList<Func<string, CancellationToken, ValueTask>>> EventHandlers = new ConcurrentDictionary<string, IList<Func<string, CancellationToken, ValueTask>>>();
    protected readonly SemaphoreSlim ReceiverSetupSemaphoreSlim = new(1);
    protected readonly SemaphoreSlim UdpClientGetSemaphoreSlim = new(1);
    protected readonly TaskFactory TaskFactory = new();

    protected UdpClient? UdpClient;
    protected CancellationTokenSource CancellationTokenSource = new();
    protected Task? Worker;

    /// <inheritdoc />
    public async ValueTask ConfigureDataReceiverAsync<TData>(Func<TData?, CancellationToken, ValueTask> handler) where TData : class
    {
        await ReceiverSetupSemaphoreSlim.WaitAsync();
        if (!EventHandlers.ContainsKey(typeof(TData).FullName ?? string.Empty))
        {
            EventHandlers.Add(typeof(TData).FullName ?? string.Empty, new List<Func<string, CancellationToken, ValueTask>>());
        }

        EventHandlers[typeof(TData).FullName ?? string.Empty].Add((d, c) => handler(d as TData, c));
        ReceiverSetupSemaphoreSlim.Release(1);
    }

    /// <inheritdoc />
    public void SetUdpClient(UdpClient? udpClient)
    {
        Logger.LogDebug($"Setting the UdpClient to {(udpClient == null ? "null" : "a value")}");
        UdpClient = udpClient;
    }

    public async ValueTask<UdpClient?> GetUdpClient(CancellationToken cancellationToken)
    {
        if (UdpClient != null)
        {
            return UdpClient;
        }

        Logger.LogDebug("Waiting on the UdpClient");
        await UdpClientGetSemaphoreSlim.WaitAsync(cancellationToken);
        while (UdpClient == null && !cancellationToken.IsCancellationRequested)
        {
            Logger.LogDebug("Still waiting on the UdpClient");
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        UdpClientGetSemaphoreSlim.Release();
        Logger.LogDebug("Got the UdpClient");
        return UdpClient;

    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Worker = TaskFactory
            .StartNew(
                async () =>
                {
                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        var client = await GetUdpClient(CancellationTokenSource.Token);
                        if (client is null)
                        {
                            continue;
                        }

                        var result = await client.ReceiveAsync(CancellationTokenSource.Token);
#pragma warning disable CS4014
                        // We want this to be fire-and-forget.
                        TaskFactory.StartNew(
#pragma warning restore CS4014
                            async () =>
                            {
                                var data = Encoding.UTF8.GetString(result.Buffer);
                                var parts = data.Split(":~:");
                                if (EventHandlers.TryGetValue(parts[0], out var handlers))
                                {
                                    var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                                    var tasks = handlers.Select(func => func(parts[1], cancellationTokenSource.Token)).ToList();
                                    while (!cancellationTokenSource.IsCancellationRequested && tasks.Any(x => !x.IsCanceled))
                                    {
                                        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationTokenSource.Token);
                                    }
                                }
                            },
                            CancellationTokenSource.Token,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Default);
                    }
                },
                CancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();
        Worker?.Dispose();
        return Task.CompletedTask;
    }
}

public interface ICentralServerPushCommunication
{
}