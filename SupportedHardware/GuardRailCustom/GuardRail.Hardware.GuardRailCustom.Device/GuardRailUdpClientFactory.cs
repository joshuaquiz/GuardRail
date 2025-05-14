using System;
using System.Net;
using System.Threading;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public sealed class GuardRailUdpClientFactory(
    IServiceProvider serviceProvider,
    ILogger<GuardRailUdpClient> logger)
{
    private GuardRailUdpClient? _guardRailUdpClient;

    public GuardRailUdpClient? GetGuardRailUdpClient() =>
        _guardRailUdpClient;

    public void InitializeGuardRailUdpClient(
        IPAddress ipAddress,
        int port)
    {
        logger.LogGuardRailDebug($"Setting {ipAddress}:{port} as the address for this device's UdpClient");
        try
        {
            _guardRailUdpClient = new GuardRailUdpClient(
                new IPEndPoint(ipAddress, port),
                logger);
            _guardRailUdpClient.OnUnMatchedRequestReceived +=
                async (udpResponse, cancellationToken) =>
                    await serviceProvider
                        .GetRequiredKeyedService<IUdpCommandHandler>(
                            udpResponse.CommandName)
                        .HandleCommand(
                            udpResponse.Body ?? string.Empty,
                            cancellationToken);
            _ = _guardRailUdpClient.StartReceivingData(
                CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogGuardRailError(e, $"Setting {ipAddress}:{port} as the address for this device's UdpClient");
            throw;
        }
    }
}