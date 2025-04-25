using System;
using GuardRail.Hardware.GuardRailCustom.Core;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class CustomHardwareSettings(
    string name,
    GuardRailUdpClient udpClient)
    : IDisposable
{
    public string Name { get; } = name;

    public GuardRailUdpClient UdpClient { get; } = udpClient;

    public void Dispose()
    {
        UdpClient.Dispose();
    }
}