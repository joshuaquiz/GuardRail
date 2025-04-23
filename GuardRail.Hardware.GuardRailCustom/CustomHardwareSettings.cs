using System.Net;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed record CustomHardwareSettings(
    string Name,
    IPEndPoint IpEndPoint);