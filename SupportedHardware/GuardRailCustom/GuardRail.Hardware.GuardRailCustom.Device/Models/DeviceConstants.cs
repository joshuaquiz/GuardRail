using System.Net;

namespace GuardRail.Hardware.GuardRailCustom.Device.Models;

public static class DeviceConstants
{
    public static string DeviceId { get; set; } = null!;

    public static IPAddress? RemoteHostIpAddress { get; set; }
}