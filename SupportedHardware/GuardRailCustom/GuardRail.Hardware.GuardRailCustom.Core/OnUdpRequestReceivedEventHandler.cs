using System.Threading;
using System.Threading.Tasks;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public delegate Task<string?> OnUdpRequestReceivedEventHandler(
    UdpResponse udpResponse,
    CancellationToken cancellationToken);