using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using GuardRail.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed class GuardRailBroadcastUdpClient : UdpClient
{
    private readonly IServiceProvider _serviceProvider;

    public GuardRailBroadcastUdpClient(
        IServiceProvider serviceProvider,
        ILogger<GuardRailBroadcastUdpClient> logger)
    {
        _serviceProvider = serviceProvider;
        this.ConfigureEncryptedTrafficLogging(logger);
    }

    /// <summary>
    /// Gets the hardware it finds on the network.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response data deserialized to the specified type.</returns>
    public async IAsyncEnumerable<(string Name, GuardRailUdpClient Client)> GetHardwareOnNetwork(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid();
        await this.SendEncryptedData(
            new IPEndPoint(IPAddress.Broadcast, GuardRailCustomConstants.UdpDiscoveryPort),
            string.Join(
                GuardRailCustomConstants.UdpSeparator,
                requestId,
                GuardRailCustomConstants.UdpCommandNames.Sync,
                GetLocalIpAddress()),
            cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            var (receivedString, receivedFrom) = await this.ReceiveEncryptedData(
                cancellationToken);
            var requestIdEnd = receivedString
                .IndexOf(
                    GuardRailCustomConstants.UdpSeparator,
                    StringComparison.Ordinal);
            if (requestIdEnd <= -1
                || !Guid.TryParse(
                    receivedString
                        .AsSpan(
                            0,
                            requestIdEnd)
                        .ToString(),
                    out var receivedRequestId)
                || receivedRequestId != requestId)
            {
                continue;
            }

            var hardwareUdpClient = new GuardRailUdpClient(
                new IPEndPoint(receivedFrom.Address, GuardRailCustomConstants.UdpCommandPort),
                _serviceProvider.GetRequiredService<ILogger<GuardRailUdpClient>>());
            hardwareUdpClient.OnUnMatchedRequestReceived +=
                async (r, ct) =>
                    await _serviceProvider
                        .GetRequiredKeyedService<IUdpCommandHandler>(
                            r.CommandName)
                        .HandleCommand(
                            r,
                            ct);
            _ = hardwareUdpClient.StartReceivingData(
                cancellationToken);
            yield return (
                Name: receivedString
                    .AsSpan(
                        requestIdEnd + 1)
                    .ToString(),
                Client: hardwareUdpClient);
        }
    }

    private static IPAddress GetLocalIpAddress() =>
        Dns.GetHostEntry(
                Dns.GetHostName())
            .AddressList
            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
        ?? throw new InvalidOperationException("No local IP address found.");
}