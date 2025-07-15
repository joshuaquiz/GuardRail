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
        string encryptionKey,
        IPEndPoint localEndPoint,
        IPEndPoint remoteEndPoint)
    {
        logger.LogGuardRailDebug($"Setting {remoteEndPoint.Address}:{remoteEndPoint.Port} as the address for this device's remote UdpClient connection");
        try
        {
            _guardRailUdpClient = new GuardRailUdpClient(
                encryptionKey,
                localEndPoint,
                remoteEndPoint,
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
            logger.LogGuardRailError(e, $"Error setting {remoteEndPoint.Address}:{remoteEndPoint.Port} as the address for this device's remote UdpClient connection");
            throw;
        }
    }
}