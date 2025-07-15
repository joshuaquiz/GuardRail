using System;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public static class DeviceConstants
{
    public static string DeviceId { get; set; } = null!;

    public static Guid? LocationId { get; set; } = Guid.NewGuid();
}