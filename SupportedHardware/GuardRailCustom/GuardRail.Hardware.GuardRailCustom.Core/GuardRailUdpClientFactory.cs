using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed class GuardRailUdpClientFactory(
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
        _ = _guardRailUdpClient.StartReceivingData(
            CancellationToken.None);
    }
}