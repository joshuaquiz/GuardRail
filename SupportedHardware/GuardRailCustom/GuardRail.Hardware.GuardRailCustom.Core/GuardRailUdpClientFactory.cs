using System;
using System.Net;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed class GuardRailUdpClientFactory(
    IServiceProvider serviceProvider,
    ILogger<GuardRailUdpClient> logger)
{
    private GuardRailUdpClient? _guardRailUdpClient;

    public GuardRailUdpClient? GetGuardRailUdpClient() =>
        _guardRailUdpClient;

    public void InitializeGuardRailUdpClient(
        IPAddress ipAddress)
    {
        _guardRailUdpClient = new GuardRailUdpClient(
            new IPEndPoint(ipAddress, GuardRailCustomConstants.UdpCommandPort),
            logger);
        _guardRailUdpClient.OnUnMatchedRequestReceived +=
            async (udpResponse, cancellationToken) =>
                await serviceProvider
                    .GetRequiredKeyedService<IUdpCommandHandler>(
                        udpResponse.CommandName)
                    .HandleCommand(
                        udpResponse,
                        cancellationToken);
        _ = _guardRailUdpClient.StartReceivingData(
            CancellationToken.None);
    }
}