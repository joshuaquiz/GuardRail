using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GuardRail.Api.Devices;
using GuardRail.Core;
using GuardRail.Core.DataModels;
using Serilog;

namespace GuardRail.Api.DeviceProviders;

public sealed class DeviceProvider : IDeviceProvider
{
    private readonly ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public DeviceProvider(
        ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns a logging device.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public Device GetDeviceByByteId(IReadOnlyList<byte> bytes)
    {
        var ints = string.Join(" ", bytes.Select(x => x.ToString("D", CultureInfo.InvariantCulture)));
        var hex = string.Join(" ", bytes.Select(x => x.ToString("X", CultureInfo.InvariantCulture)));
        _logger.Debug($"A request was made for a device (ID as ints {ints}) (ID as hex {hex})");
        return new LoggerDevice();
    }
}