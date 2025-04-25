using System.Net;

namespace GuardRail.Hardware.GuardRailCustom.Core;

public sealed class HardwareDiscoveryPacket
{
    public required int Port { get; set; }

    public required IPAddress IpAddress { get; set; }

    public required string Name { get; set; }

    public required bool IsRunningOnDevice { get; set; }
}