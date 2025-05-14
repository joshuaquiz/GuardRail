using System;
using System.Collections.Generic;
using GuardRail.Hardware.GuardRailCustom.Core;

namespace GuardRail.Hardware.GuardRailCustom;

public sealed class CustomHardwareSettings(
    string name,
    GuardRailUdpClient udpClient)
    : IEqualityComparer<CustomHardwareSettings>, IDisposable
{
    public string Name { get; } = name;

    public GuardRailUdpClient UdpClient { get; } = udpClient;

    public void Dispose()
    {
        UdpClient.Dispose();
    }

    public bool Equals(CustomHardwareSettings? x, CustomHardwareSettings? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name
               && x.UdpClient.RemoteEndPoint.Equals(y.UdpClient.RemoteEndPoint);
    }

    public int GetHashCode(CustomHardwareSettings obj)
    {
        return HashCode.Combine(obj.Name, obj.UdpClient);
    }
}