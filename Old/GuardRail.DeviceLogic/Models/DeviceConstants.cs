using System;
using System.Net;

namespace GuardRail.DeviceLogic.Models;

public static partial class DeviceConstants
{
    public static Guid DeviceId { get; set; }

    public static IPAddress? RemoteHostIpAddress { get; set; }
}